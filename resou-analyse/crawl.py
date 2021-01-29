import requests
import fake_useragent
import os, json, sys, time
import datetime, random, base64
from const import DEFAULT_DATABASE
from const import TABLE_TIME_ID
from const import TABLE_TRENDING
from store import MongoStore


# 初始化MongoDB
store = MongoStore(DEFAULT_DATABASE)

# 读取cookie
def read_cookie():
    with open('cookie.txt', 'rt', encoding='utf-8') as fp:
        return ''.join(fp.readlines())

# 构造日期序列
def build_dates(begin, days):
    delta = datetime.timedelta(days=1)
    for i in range(days):
        yield begin + delta * i

# 执行请求
def request(url, cookie):
    headers = {
        "User-Agent": fake_useragent.FakeUserAgent().random, 
        "Cookie": cookie
    }
    response = requests.get(url, headers=headers)
    response.raise_for_status()
    response.encoding = response.apparent_encoding
    return response.content

# 获取指定日期对应的timeId
def get_timeId(date, cookie):
    cacheKey = date.strftime('%Y-%m-%d')
    records = list(store.find(TABLE_TIME_ID, {'date': cacheKey}))
    if len(records) > 0:
       return records[0]['timeId']
    else:
        data = "[\"getclosesttime\",[\"{d}\"]]".format(d=cacheKey)
        data = base64.b64encode(data.encode('utf-8'))
        url = 'https://www.weibotop.cn/apis/androidrouter/?versioncode=1&=&data=' + str(data, 'utf-8')
        data = request(url, cookie)
        timeId = json.loads(data)[0]
        store.insert(TABLE_TIME_ID, [{'date': cacheKey, 'timeId': timeId }])
        return timeId

# 获取指定timeId对应的热搜
def get_weibo_trending(timeId, cookie):
    records = list(store.find(TABLE_TRENDING, {'timeId': timeId}))
    if len(records) > 0:
        return records
    else:
        url = 'https://www.eecso.com/test/weibo/apis/currentitems.php?timeid=' + timeId
        data = request(url, cookie)
        data = json.loads(data)
        trendings = list(map(lambda x:{'title':x[0], 'createdDate':x[1], 'updatedDate':x[2], 'rank':int(x[3])}, data))
        for trending in trendings:
            trending['timeId'] = timeId
            trending['href'] = 'https://s.weibo.com/weibo?q=' + trending['title']
            trending['createdDate'] = datetime.datetime.strptime(trending['createdDate'], '%Y-%m-%d %H:%M:%S')
            trending['updatedDate'] = datetime.datetime.strptime(trending['updatedDate'], '%Y-%m-%d %H:%M:%S')
        store.insert(TABLE_TRENDING, trendings)
        return trendings

# 获取指定日期范围内的热搜
def get_weibo_trendings(begin, days, cookie):
    dates = build_dates(begin, days)
    for date in dates:
        timeId = get_timeId(date, cookie)
        print(f'{date} -> {timeId}')
        trendings = get_weibo_trending(timeId, cookie)
        print(list(map(lambda x:x['title'], trendings)))
        time.sleep(1)


if __name__ == '__main__':
   if len(sys.argv) < 3:
      print('eg. python crawl.py 2021-01-01 366')
      return
      
    # 抓取指定时间段内的微博热搜
    begin = datetime.datetime.strptime(sys.argv[1], '%Y-%m-%d')
    days = int(sys.argv[2])
    cookie = read_cookie()
    get_weibo_trendings(begin, days, cookie)


