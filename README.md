# bank-accounts
Задание стажировки C# в Модульбанк:
микросервис «Банковские счета», обслуживающий процессы розничного банка.

## 🚀 Запуск проекта
```bash
  docker compose up --build
```
Документация Swagger будет доступна по адресу: 

http://0.0.0.0:8080/swagger/index.html

## Получение JWT токена

Для получения JWT токена необходимо выполнить POST-запрос на эндпоинт:

http://localhost:8081/realms/BankAccount/protocol/openid-connect/token

bash
Копировать
Редактировать

### Заголовки
- `Content-Type: application/x-www-form-urlencoded`

### Тело запроса (form-data или x-www-form-urlencoded):
| Параметр      | Значение           | Описание               |
|---------------|--------------------|------------------------|
| grant_type    | client_credentials | Тип получения токена   |
| client_id     | AccountClient      | Идентификатор клиента  |
| client_secret | my-secret          | Секрет клиента         |

### Пример ответа (успешный `HTTP 200 OK`):
```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires_in": 300,
  "refresh_expires_in": 0,
  "token_type": "Bearer",
  "not-before-policy": 0,
  "scope": "profile email"
}