import os
import gc
import threading
from PIL import Image, ImageDraw, ImageFont
import time
import datetime
import cv2
import numpy as np
from pyzbar.pyzbar import decode
import winsound
from paddleocr import PaddleOCR
from pyzbar.wrapper import ZBarSymbol
from multiprocessing import Process, Manager

# 颜色范围定义
color_dist = {
    'red': {'Lower': np.array([0, 60, 60]), 'Upper': np.array([6, 255, 255])},
    'blue': {'Lower': np.array([100, 80, 46]), 'Upper': np.array([124, 255, 255])},
    'green': {'Lower': np.array([35, 43, 35]), 'Upper': np.array([90, 255, 255])},
    'golden': {'Lower': np.array([26, 43, 46]), 'Upper': np.array([34, 255, 255])},
}

# 标记线条颜色
color_marker = (0, 0, 255)
font_marker = './fonts/msyh.ttc'

# PaddleOCR
ocr = PaddleOCR() 

# 检测图块
def detect_blocks(image):
    blocks = []
    (img_h, img_w, _) = image.shape
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    _, binary = cv2.threshold(gray, 135, 255, cv2.THRESH_BINARY)
    cv2.imwrite('./binary.jpg', binary)
    contours = cv2.findContours(binary, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)[-2] 
    for i in range(len(contours)):
        block_rect = cv2.boundingRect(contours[i]) 
        x,y,w,h = block_rect
        if w > (img_w / 2) and w != img_w and h != img_h:
           block_image = image[y:y+h, x:x+w]
           block_file = f'./blocks/block_{i}.jpg' 
           cv2.imwrite(block_file, block_image)
           blocks.append((block_image, block_rect, block_file))
    
    blockHeight = sum(list(map(lambda x:x[1][3], blocks)))
    blockHeight = img_h - blockHeight - 105
    block_rect = (0, 0, img_w, blockHeight)
    block_image = image[0:blockHeight, 0:img_w]
    block_file = f'./blocks/block_{len(blocks)}.jpg' 
    cv2.imwrite(block_file, block_image)
    blocks.append((block_image, block_rect, block_file))
    return blocks

# 处理图块
def handle_block(image, block):
    _, block_rect, block_file = block
    block_x, block_y, block_w, block_h = block_rect
    cv2.rectangle(image, (block_x, block_y), (block_x + block_w, block_y + block_h), color_marker, 2)
    flag, data, rect = detect_qrcode(image, block)
    if flag:
        x,y,w,h = rect
        print('二维码信息：' + data)

        # 检测二维码颜色区域
        qrcode = image[y:y+h, x:x+w]
        flag = detect_color(qrcode,'green')
        print('防疫信息：' + ('正常' if flag else '异常'))

        # 检测疫苗颜色区域
        img_w = image.shape[1]
        qrcode = image[y:y+h, x+w:img_w]
        flag = detect_color(qrcode,'golden')
        print('疫苗信息：' + ('已注射' if flag else '未注射'))
    else:
        texts = list(detect_text(image, block))
        print(texts)

    os.remove(block_file)
    return image

# 检测二维码
def detect_qrcode(image, block):
    block_image, block_rect, _ = block
    block_x, block_y, _, _ = block_rect
    gray = cv2.cvtColor(block_image, cv2.COLOR_BGR2GRAY)
    qrcodes = decode(gray, [ZBarSymbol.QRCODE])
    if len(qrcodes) > 0:
        qrcode = qrcodes[0]
        qrcodeData = qrcode.data.decode("utf-8")
        x, y, w, h = qrcode.rect
        abs_x = block_x + x
        abs_y = block_y + y
        cv2.rectangle(image, (abs_x, abs_y), (abs_x + w, abs_y + h), color_marker, 2)
        return True, qrcodeData, (abs_x, abs_y, w, h)
    else:
        return False, None, None

# 检测颜色
def detect_color(image, color):
    # gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY) # 灰度
    gs = cv2.GaussianBlur(image, (5, 5), 0)  # 高斯模糊
    hsv = cv2.cvtColor(gs, cv2.COLOR_BGR2HSV)  # HSV
    erode_hsv = cv2.erode(hsv, None, iterations=2) # 腐蚀
    inRange_hsv = cv2.inRange(erode_hsv, color_dist[color]['Lower'], color_dist[color]['Upper'])
    contours = cv2.findContours(inRange_hsv.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)[-2]
    if len(contours) > 0:
        draw_color_area(image, contours)
        return True
    else:
        winsound.Beep(440, 5000)
        return False

# 标记颜色区域
def draw_color_area(image, contours):
    max, index = 0, -1
    for i in range(len(contours)):
        area = cv2.contourArea(contours[i])
        if area > max:
            max = area
            index = i
    if index >= 0:
        rect = cv2.minAreaRect(contours[index])
        cv2.ellipse(image, rect, color_marker, 2, 8)
        cv2.circle(image, (np.int32(rect[0][0]), np.int32(rect[0][1])), 2, color_marker, 2, 8, 0)

# 检测文字
def detect_text(image, block):
    _, block_rect, block_file = block
    block_x, block_y, _, _ = block_rect
    result = ocr.ocr(block_file)
    for line in result:
        boxes = line[0]
        texts = line[1][0]
        x = int(boxes[0][0])
        y = int(boxes[0][1])
        w = int(boxes[2][0]) - x
        h = int(boxes[2][1]) - y
        abs_x = block_x + x
        abs_y = block_y + y
        cv2.rectangle(image, (abs_x, abs_y), (abs_x + w, abs_y + h), color_marker, 2)
        yield texts

# 绘制文字
def draw_text(image, text, position, textColor=(0, 255, 0), textSize=30):
    if (isinstance(image, np.ndarray)):
        img = Image.fromarray(cv2.cvtColor(image, cv2.COLOR_BGR2RGB))
    draw = ImageDraw.Draw(img)
    fontStyle = ImageFont.truetype(font_marker, textSize, encoding="utf-8")
    draw.text(position, text, textColor, font=fontStyle)
    return cv2.cvtColor(np.asarray(img), cv2.COLOR_RGB2BGR)

# 处理静态图片
def handle_image(image, scale = 1.0):

    # 检测画面中的图块
    blocks = list(detect_blocks(image))
        
    # 处理每个图块
    for block in blocks:
        image = handle_block(image, block)
        
    # 展示处理结果
    (img_h, img_w, _) = image.shape
    image = cv2.resize(image, (int(img_w * scale), int(img_h * scale)))
    cv2.imshow('QRCode Detecting', image)
    cv2.waitKey(0)

# 处理视频
def handle_video():
    cap = cv2.VideoCapture(0)
    while True:
        ret, image = cap.read()
        if ret:
           # 检测画面中的图块
            blocks = list(detect_blocks(image))
        
            # 处理每个图块
            for block in blocks:
                image = handle_block(image, block)
        
            # 展示处理结果
            # timestamp = datetime.datetime.now().strftime("%Y-%m-%dT%H:%M:%S")
            # frame_file = f'C:\\Users\\XA-162\\Desktop\\frames\\{timestamp}.jpg'
            # cv2.imwrite(frame_file, image)
            (img_h, img_w, _) = image.shape
            image = cv2.resize(image, (int(img_w * 1.0), int(img_h * 1.0)))
            cv2.imshow('QRCode Detecting', image)

            # 按 Q 退出
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break
        else:
            continue

    cap.release() 
    cv2.destroyAllWindows()


if __name__ == "__main__":

    if not os.path.exists('./blocks'):
        os.mkdir('./blocks')

    if not os.path.exists('./frames'):
        os.mkdir('./frames')
    
    # 处理静态图片
    # image = cv2.imread('./test.jpg')
    # handle_image(image, 0.3)
    
    # 处理动态视频
    handle_video()



