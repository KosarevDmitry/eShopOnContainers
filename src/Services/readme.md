why do  `location and marketing`  no have sources


# Basket.API

all operation were handled in redis db

UserCheckoutAcceptedIntegrationEvent
получив  rest запрос port 80 формирует event и публикует его для ordering.api 

получает из шины
ProductPriceChangedIntegrationEvent и вызывает  update redis
OrderStartedIntegrationEvent и вызывает delete by userId

такие же операции обновления, удаления получает по rest 80 port
и по grps port 5001

интересно 
`failed` path url для остановки или возобновления обработки запросов

oidc-token-manager.js
обработчик ошибок  единственный


Identity.API
  "MvcClient": "http://localhost:5100",
  "SpaClient": "http://localhost:5104",
   "XamarinCallback": "http://localhost:5105/xamarincallback",