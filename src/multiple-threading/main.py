import requests
import functools
import os
import sys
import uuid
import json
import datetime
import random
import threading
import threadpool
import multiprocessing
import fake_useragent
import concurrent
import asyncio
from concurrent.futures import ThreadPoolExecutor
from concurrent.futures import ProcessPoolExecutor
from concurrent.futures import wait, ALL_COMPLETED, FIRST_COMPLETED


class Spider:

    def __init__(self, urls):
        self.session = requests.session()
        self.session.headers['User-Agent'] = fake_useragent.UserAgent().random
        self.session.headers["Referer"] = "https://www.nvshens.org"
        self.urls = urls

    # 下载图片
    def getImage(self, url, fileName, retries=5):
        try:
            print(f'{threading.currentThread().name} -> {url}')
            response = self.session.get(
                url, allow_redirects=False, timeout=10, proxies=None)
            response.raise_for_status()
            data = response.content
            imgFile = open(fileName, 'wb')
            imgFile.write(data)
            imgFile.close()
            return True
        except:
            while retries > 0:
                retries -= 1
                if self.getImage(url, fileName, retries):
                    break
                else:
                    continue
    
    async def getImageAsync(self, url, fileName, retries=5):
        try:
            print(f'{threading.currentThread().name} -> {url}')
            headers = {
                'User-Agent': fake_useragent.UserAgent().random,
                'Referer': "https://www.nvshens.org"
            }
            future = asyncio.get_event_loop().run_in_executor(
                None, 
                functools.partial(requests.get, url, headers=headers)
            )
            response = await future
            data = response.content
            imgFile = open(fileName, 'wb')
            imgFile.write(data)
            imgFile.close()
            return True
        except:
            while retries > 0:
                retries -= 1
                if await self.getImageAsync(url, fileName, retries):
                    break
                else:
                    continue

    # 使用Thread下载
    def downloadByThread(self):
        threads = []
        for index in range(0, len(self.urls)):
            thread = threading.Thread(target=self.getImage, args=(
                self.urls[index], f'{str(index)}.jpg',))
            threads.append(thread)

        for thread in threads:
            thread.setDaemon(True)
            thread.start()

    def downloadByProcess(self):
        process = []
        for index in range(0, len(self.urls)):
            proc = multiprocessing.Process(target=self.getImage, args=(
                self.urls[index], f'{str(index)}.jpg',))
            process.append(proc)

        for proc in process:
            proc.start()


    # 使用ThreadPool下载
    def downloadByThreadPool(self, poolSize=3):
        count = len(self.urls)
        # 构造线程参数
        args = []
        for index in range(0, count):
            args.append(
                (None, {'url': self.urls[index], 'fileName': f'{str(index)}.jpg'}))
        # 线程池大小
        if count < poolSize:
            poolSize = count
        # 构造线程池
        pool = threadpool.ThreadPool(poolSize)
        requests = threadpool.makeRequests(self.getImage, args)
        [pool.putRequest(req) for req in requests]
        pool.wait()

    def downloadByProcessPool(self, poolSize=3):
        count = len(self.urls)
        # 构造线程参数
        args = []
        for index in range(0, count):
            args.append((self.urls[index], f'{str(index)}.jpg', ))
        # 线程池大小
        if count < poolSize:
            poolSize = count
        # 构造线程池
        pool = multiprocessing.Pool(poolSize)
        for arg in args:
            pool.apply(self.getImage, arg)

    # 使用ThreadPoolExecutor下载
    def downloadByThreadPoolExecutor(self, poolSize=3):
        count = len(self.urls)
        # 构造线程参数
        args = []
        for index in range(0, count):
            args.append({'url': self.urls[index], 'fileName': f'{str(index)}.jpg'})
        # 线程池大小
        if count < poolSize:
            poolSize = count
        # 构造线程池
        pool = ThreadPoolExecutor(max_workers=poolSize)
        tasks = []
        for arg in args:
            task = pool.submit(self.getImage(arg['url'], arg['fileName']), arg)
            tasks.append(task)
        wait(tasks, return_when=ALL_COMPLETED)
    # tasks = pool.map(lambda arg:self.getImage(arg['url'], arg['fileName']), args)

    # 使用ProcessPoolExecutor
    def downloadByProcessPoolExecutor(self, poolSize=3):
        count = len(self.urls)
        # 构造线程参数
        args = []
        for index in range(0, count):
            args.append({'url': self.urls[index], 'fileName': f'{str(index)}.jpg'})
        # 线程池大小
        if count < poolSize:
            poolSize = count
        # 构造线程池
        pool = ProcessPoolExecutor(max_workers=poolSize)
        for arg in args:
            pool.submit(self.getImage(arg['url'], arg['fileName']), arg)

    async def downloadAsync(self):
        tasks = []
        count = len(self.urls)
        for index in range(0, count):
            url = self.urls[index]
            fileName = f'{str(index)}.jpg'
            await asyncio.get_event_loop().create_task(self.getImageAsync(url, fileName))
            # task = asyncio.ensure_future(self.getImageAsync(url,fileName))
            # tasks.append(task)
        # for task in asyncio.as_completed(tasks):
        #     await task


async def say_after(what, delay):
    await asyncio.sleep(delay)
    print(what)

async def main():
    await say_after('你好', 1)
    await say_after('Hello', 2)

# Python 3.7 + 
# asyncio.rum(main())
# Python 3.7 -
# asyncio.get_event_loop().run_until_complete(main())

async def main2():
    task1 = asyncio.get_event_loop().create_task(say_after('你好', 1))
    task2 = asyncio.get_event_loop().create_task(say_after('Hello', 2))
    await task1
    await task2

# asyncio.get_event_loop().run_until_complete(main2())

urls = [
    'https://img.onvshen.com:85/gallery/25249/28847/010.jpg',
    'https://img.onvshen.com:85/gallery/25249/28847/011.jpg',
    'https://img.onvshen.com:85/gallery/25249/28847/012.jpg',
    'https://img.onvshen.com:85/gallery/25249/28847/013.jpg',
    'https://img.onvshen.com:85/gallery/25249/28847/014.jpg',
    'https://img.onvshen.com:85/gallery/25249/28847/015.jpg',
]


async def crawler(url):
    print('Start crawling:', url)
    headers = {'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36'}
    
    # 利用BaseEventLoop.run_in_executor()可以在coroutine中执行第三方的命令，例如requests.get()
    # 第三方命令的参数与关键字利用functools.partial传入
    future = asyncio.get_event_loop().run_in_executor(None, functools.partial(requests.get, url, headers=headers))
    
    response = await future
     
    print('Response received:', url)
    # 处理获取到的URL响应（在这个例子中我直接将他们保存到硬盘）
    with open(os.path.join('.', url.split('/')[-1]), 'wb') as output:
        output.write(response.content)

# loop = asyncio.get_event_loop()
# tasks = [crawler(url) for url in urls]
# asyncio.get_event_loop().run_until_complete(asyncio.wait(tasks))
# # loop.close()

if __name__ == '__main__':
    spider = Spider(urls)
    start = datetime.datetime.now()
    # print('downloadByThread...')
    # spider.downloadByThread()

    loop = asyncio.get_event_loop()
    task = loop.create_task(spider.downloadAsync())
    loop.run_until_complete(task)
    end = datetime.datetime.now()
    print(f'finished in {end -start}')
    # task = spider.downloadAsync()
    # task = asyncio.get_event_loop().create_task(spider.downloadAsync())
    # asyncio.get_event_loop().run_until_complete(task)
    # print(f'finished in {end -start}')

    # print('-------------------------')

    # print('downloadByProcess...')
    # spider.downloadByProcess()
    # print('downloadByProcessPool...')
    # spider.downloadByProcessPool()

    # start = datetime.datetime.now()
    # print('downloadByThreadPool...')
    # spider.downloadByThreadPool()
    # end = datetime.datetime.now()
    # print(f'finished in {end -start}')

    # print('-------------------------')

    # start = datetime.datetime.now()
    # print('downloadByThreadPoolExecutor...')
    # spider.downloadByThreadPoolExecutor()
    # end = datetime.datetime.now()
    # print(f'finished in {end -start}')

    # print('-------------------------')

    # # start = datetime.datetime.now()
    # print('downloadByProcessPoolExecutor...')
    # spider.downloadByProcessPoolExecutor()
    # end = datetime.datetime.now()
    # print(f'finished in {end -start}')
