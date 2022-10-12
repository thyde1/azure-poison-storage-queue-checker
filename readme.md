### Usage
Create user secrets in the format

```json
{
  "StorageAccounts": [
    {
      "Name": "{{ACCOUNT DISPLAY NAME}}",
      "ConnectionString": "{{STORAGE ACCOUNT CONNECTION STRING}}"
    },
    {
      "Name": "{{ACCOUNT DISPLAY NAME}}",
      "ConnectionString": "{{STORAGE ACCOUNT CONNECTION STRING}}"
    }
  ],
  "SkipLoggingMessages": false,
  "SkipAskingForConnectionString": false
}
```

If you want to use StorageAccounts setting from the user secrets, set `SkipAskingForConnectionString` to `true`. Otherwise it will ask you to input it at the start of the execution instead.

If you want to just log the names of the non empty poison queues instead of logging full messages set `SkipLoggingMessages` to `true`.

Run and have fun!