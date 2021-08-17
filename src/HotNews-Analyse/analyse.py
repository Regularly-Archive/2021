from collections import Counter
from itertools import groupby
from bs4 import BeautifulSoup
import jieba
import json, datetime
from jieba import analyse
import jieba.posseg as pseg
from pyecharts.charts import WordCloud
import pyecharts.options as opts
from pyecharts.charts import Line, Bar, Timeline, Grid
from pyecharts.faker import Faker
from const import DEFAULT_DATABASE
from const import TABLE_TRENDING
from store import MongoStore
from snownlp import SnowNLP
import matplotlib
matplotlib.use('Agg')
from matplotlib import pyplot as plt
import numpy as np

# 初始化Matplotlib
plt.rcParams['font.sans-serif']=['SimHei']
plt.rcParams['axes.unicode_minus']=False

# 初始化MongoDB
store = MongoStore(DEFAULT_DATABASE)

# 按月份组织热搜
def extract_month_trendings(trendings):
    month_trendings = {}
    for month, group in groupby(trendings, key=lambda x: x['createdDate'].month):
        events = list(group)
        if month in month_trendings.keys():
            month_trendings[month].extend(events)
        else:
            month_trendings[month] = events
    return month_trendings

# 分析热搜关键词
def analyse_trending_tags(month_trendings):
    yearly_tags = []
    for month, events in sorted(month_trendings.items()):
        titles = list(sorted(events, key=lambda x: x['rank'], reverse=True))
        fullTitles = ';'.join(map(lambda x: x['title'], titles))
        tags = analyse.extract_tags(fullTitles, topK=100, withWeight=True, allowPOS=('ns', 'n', 'vn', 'v'))
        tags = list(map(lambda x: x[0], tags))
        yearly_tags.extend(tags)
    draw_tags(yearly_tags, '2020年微博热搜词云')

# 全年话题热度分析
def analyse_trending_ranks(month_trendings):
    ranks_data = {}
    for month, events in sorted(month_trendings.items()):
        totalRanks = sum(list(map(lambda x:x['rank'], list(events))))
        totalRanks = round(totalRanks / 10000000, 2)
        ranks_data[str(month)] = totalRanks
    labels = list(ranks_data.keys())
    values = list(ranks_data.values())
    x = np.arange(len(labels))
    width = 0.35
    fig, ax = plt.subplots()
    rect = ax.bar(x - width/2, values, width, label='话题热度')
    ax.set_ylabel('话题热度(亿)')
    ax.set_title('2020全年微博热搜热度变化趋势')
    ax.set_xticks(x)
    ax.set_xticklabels(labels)
    ax.legend()
    for item in rect:
        height = item.get_height()
        ax.annotate('{}'.format(height),
            xy=(item.get_x() + item.get_width() / 2, height),
            xytext=(0, 3),
            textcoords="offset points",
            ha='center', va='bottom'
        )
    plt.savefig('./Reports/2020全年微博热搜热度变化趋势.jpg')
    plt.show()

# 全年话题数分析
def analyse_trending_count(month_trendings):
    counts_data = {}
    for month, events in sorted(month_trendings.items()):
        counts_data[str(month)] = len(list(events))
    labels = list(counts_data.keys())
    values = list(counts_data.values())
    x = np.arange(len(labels))
    width = 0.35
    fig, ax = plt.subplots()
    rect = ax.bar(x - width/2, values, width, label='话题数量')
    ax.set_ylabel('话题数量(个)')
    ax.set_title('2020全年微博热搜数量变化趋势')
    ax.set_xticks(x)
    ax.set_xticklabels(labels)
    ax.legend()
    for item in rect:
        height = item.get_height()
        ax.annotate('{}'.format(height),
            xy=(item.get_x() + item.get_width() / 2, height),
            xytext=(0, 3),
            textcoords="offset points",
            ha='center', va='bottom'
        )
    plt.savefig('./Reports/2020全年微博热搜数量变化趋势.jpg')
    plt.show()

# 提取全年话题人物
def extract_character_from_trending(trendings):
    characters = { }
    topics = list(filter(lambda x:'特朗普' in x['title'], trendings))
    topics = list(map(lambda x:{'title':x['title'], 'date':x['createdDate'].strftime('%Y-%m-%d %H:%M:%S')}, topics))
    with open('results.json','wt',encoding='utf-8') as fp:
        json.dump(topics, fp)
    for trending in trendings:
        title = trending['title']
        words = [x.word for x in  pseg.cut(title) if x.flag == 'nr']
        if len(words) > 0:
           for word in words:
               if (word in characters.keys()):
                   characters[word] += 1
               else:
                   characters[word] = 1
    characters = dict(sorted(characters.items(), key=lambda x:x[1], reverse=True)[:10])
    c = (
        Bar()
        .add_xaxis(list(characters.keys()))
        .add_yaxis("上榜次数", list(characters.values()))
        .set_global_opts(
            title_opts=opts.TitleOpts(title="2020全年微博热搜上榜人物分析", pos_left=325),
            legend_opts=opts.LegendOpts(type_="scroll", pos_left="left", orient="vertical"),
            xaxis_opts=opts.AxisOpts(axislabel_opts=opts.LabelOpts(rotate=-20)),
        )
        .render("./Reports/2020全年微博热搜上榜人物分析.html")
    )

# 分析全年情感变化趋势
def analyse_trending_sentiment(trendings):
    sentiments = {}
    base_date = datetime.datetime.strptime('2020-01-01', '%Y-%m-%d')
    for trending in trendings:
        nlp = SnowNLP(trending['title'])
        day = (trending['createdDate'] - base_date).days + 1
        if day in sentiments.keys():
           sentiments[day].append(nlp.sentiments)
        else:
           sentiments[day] = [nlp.sentiments]
    for day, list in  sentiments.items():
        sentiments[day] = sum(list) / len(list)
    values = list(map(lambda x:x[1], sentiments.items()))
    x = range(len(values))
    fig, ax = plt.subplots()
    plt.plot(x, values)
    ax.set_ylabel('情感打分')
    ax.set_title('2020全年微博热搜情感变化趋势')
    ax.legend()
    plt.savefig('./Reports/2020全年微博热搜情感变化趋势.jpg')
    plt.show()
        
# 绘制词云
def draw_tags(words, title):
    words = list(filter(lambda x: x != '', words))
    data = Counter(words)
    c = (
        WordCloud()
        .add(series_name="热门词汇", data_pair=data.items(), word_size_range=[6, 66])
        .set_global_opts(
            title_opts=opts.TitleOpts(
                title=title, title_textstyle_opts=opts.TextStyleOpts(font_size=23)
            ),
            tooltip_opts=opts.TooltipOpts(is_show=True),
        )
        .render('.\Reports\{title}.html'.format(title=title))
    )


trendings = store.find(TABLE_TRENDING, {})
analyse_trending_sentiment(list(trendings))
month_trendings = extract_month_trendings(list(trendings))
analyse_trending_ranks(month_trendings)
analyse_trending_count(month_trendings)
extract_character_from_trending(list(trendings))
analyse_trending_tags()
