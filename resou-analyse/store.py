import pymongo
from const import MONGO_URL

class MongoStore(object):
    def __init__(self, database):
        self.client = pymongo.MongoClient(MONGO_URL)
        self.context = self.client[database]

    def insert(self, table, objs):
        self.context[table].insert_many(objs, ordered=False)
    
    def find(self, table, query):
        return self.context[table].find(query)

