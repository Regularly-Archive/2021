from snownlp import sentiment
sentiment.train('./train/neg60000.txt', './train/pos60000.txt')
sentiment.save('weibo.marshal')