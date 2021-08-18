import cv2
import numpy as np
from pyzbar.pyzbar import decode
import winsound

# 颜色范围定义
color_dist = {
    'red': {'Lower': np.array([0, 60, 60]), 'Upper': np.array([6, 255, 255])},
    'blue': {'Lower': np.array([100, 80, 46]), 'Upper': np.array([124, 255, 255])},
    'green': {'Lower': np.array([35, 43, 35]), 'Upper': np.array([90, 255, 255])},
    'yellow': {'Lower': np.array([26, 43, 46]), 'Upper': np.array([34, 255, 255])},
}

# 检测二维码
def detect_qrcode(image):
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    qrcodes = decode(gray)
    if len(qrcodes) > 0:
        qrcode = qrcodes[0]
        qrcodeData = qrcode.data.decode("utf-8")
        x,y,w,h = qrcode.rect
        cv2.rectangle(image, (x, y), (x + w, y + h), (0, 255, 0), 2)
        return True, qrcodeData, qrcode.rect
    else:
        return False, None, None

# 检测颜色
def detect_color(image, color):
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY) # 灰度
    gs = cv2.GaussianBlur(gray, (5, 5), 0)  # 高斯模糊
    hsv = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)  # HSV
    erode_hsv = cv2.erode(hsv, None, iterations=2) # 腐蚀
    inRange_hsv = cv2.inRange(erode_hsv, color_dist[color]['Lower'], color_dist[color]['Upper'])
    contours = cv2.findContours(inRange_hsv.copy(), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)[-2]
    if (len(contours)) > 0:
        draw_color_area(image, contours)
    else:
        winsound.Beep(440, 5000)

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
        cv2.ellipse(image, rect, (0, 255, 0), 2, 8)
        cv2.circle(image, (np.int32(rect[0][0]), np.int32(rect[0][1])), 2, (0, 255, 0), 2, 8, 0)

if __name__ == "__main__":
    image_path = 'test.jpg'
    image = cv2.imread(image_path)
    (h, w, _) = image.shape
    image = cv2.resize(image, (int(w * 0.3), int(h * 0.3)))
    flag, data, rect = detect_qrcode(image)
    if flag:
        x,y,w,h = rect
        print('二维码信息：' + data)
        qrcode = image[y:y+h, x:x+w]
        detect_color(qrcode,'green')
        #detect_color(qrcode,'yellow')
    else:
        winsound.Beep(440, 5000)

    cv2.imshow('QRCode Detecting', image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()
