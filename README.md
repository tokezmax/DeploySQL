# DeploySQL
自動化佈屬MS-SQL腳本工具，
並可彈性制定你的執行策略


# 目地(Objective)
若你的軟件的佈屬，
需要運用到大量的SQL腳本語法，
但卻有因為環境不同而有些許的差異化，

你只需要編輯一件json文檔，
就可以隨意安排sql腳本的執行順序。
  
  
# 如何使用 (How to use)
※你需要先準備好一個資料庫，並準備好資料庫連線字串，確保此帳號有db_onwer的權限

1. 設定「Z_Config.json」

```json
{
  "AutoCloseConsole": false, 
  "LogPath": "Logs",
  "DbRetryCount": 5,
  "Project": [
    {
      "Name": "localhost-db-dev",
      "ScriptPath": "database\\opmc-db",
      "ConnectString": "ConnectString....",
      "Strategy": [
        "base",
        "Dev"
      ],
      "ignoreFile": [],
      "ClearAllSp": false,
      "ClearAllTable": false
    }
  ],
  "ActPlans": [
    {
      "Name": "base",
      "RunDowns": [
        "CreateTable",
        "InitData",
		"Views",
		"Sp"
      ]
    },
    {
      "Name": "Dev",
      "RunDowns": [ "Env\\Dev" ]
    },
    {
      "Name": "Qat",
      "RunDowns": [ "Env\\Qat" ]
    }
  ]
}
```
Z_Config.json

2.打開命令提示字元(CLI)，輸入下列指令(記得用Admin身份開啟)
Z_DeploySQL.exe <Project>
  <Project>: 專案名稱 (關聯檔案「Z_Config.json」)


  



