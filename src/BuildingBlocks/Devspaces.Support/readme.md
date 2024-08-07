Хороший пример с точки зрения устройства сервисов
каждый класс должен быть добавлен  в коллекцию сервисов
это естественный способ его создания
потом передавай куда уже требуется
`services.AddTransient<DevspacesMessageHandler>();`

`builder.AddHttpMessageHandler<DevspacesMessageHandler>();`

проект демонстрирует как добавить
подключить обработчик сообщения DelegatingHandler