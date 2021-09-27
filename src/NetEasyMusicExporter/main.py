import os
import sys
import shutil
import requests
import json
from lxml import etree
import logging

logging.basicConfig(level = logging.INFO, format = '%(asctime)s - %(name)s - %(levelname)s - %(message)s')
logger = logging.getLogger('NetEasyMusicExporter')

def resolve(url, timeout):
    try:
        headers = {
          'Accept': '',
          'User-Agent': 'apifox/1.0.0 (https://www.apifox.cn)'
        }

        response = requests.get(url, timeout=timeout, headers=headers)
        response.raise_for_status()
        return parseContent(response.content)
    except Exception as e:
        logger.error(f'resolve data from {url} fails due to offline', exc_info=True)
        return None

def parseContent(content):
    html = etree.HTML(content)
    items = html.xpath('//table[@class="m-table"]')
    next = html.xpath('string(//span[@class="next"]/a/@href)')
    next = str(next)
    if next.startswith('/'):
        next = f'https://movie.douban.com{next}'

def crawl(uid, timeout):
    url = f'https://music.163.com/#/my/m/music/playlist?id=42062293'
    resolve(url, timeout)

if __name__ == '__main__':
    crawl('', 180)