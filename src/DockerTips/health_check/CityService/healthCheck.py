import requests

response = requests.get('http://localhost:8081/city')
if response.status_code == 200:
    exit(0)
else:
    exit(1)
