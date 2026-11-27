# 金融商品喜好紀錄系統

使用者可新增、查詢、修改、刪除喜好的金融商品，系統自動計算總手續費用與預計扣款總金額。

## 快速啟動
先將 .env.example 轉換成 .env檔案
接著只需安裝 [Docker Desktop](https://www.docker.com/products/docker-desktop/)。

```bash
docker compose up -d --build
```

啟動後開啟 http://localhost:8080

首次啟動會自動建立資料庫 `FPPR` 並執行 `DB/` 內的腳本（約 30 秒）。

| 服務 | 說明 |
|---|---|
| `db` | SQL Server 2022，`localhost,1433`，帳號 `sa`，密碼見 `.env` |
| `db-init` | 依序執行 `DDL.sql` → `StoredProcedure.sql` → `DML.sql`，資料庫已存在則略過 |
| `web` | ASP.NET MVC 網站 |

重建資料庫：`docker compose down -v && docker compose up -d --build`

## 技術

- C#、ASP.NET Core 6 MVC、Razor、Bootstrap 5（RWD）
- SQL Server、Stored Procedure、Dapper
- xUnit

## 架構

```
FPPR_System             展示層：Controller、Razor View
FPPR_System.Business    業務層：輸入驗證、錯誤處理
FPPR_System.DataAccess  資料層：透過 Dapper 呼叫 Stored Procedure
FPPR_System.Common      共用層：各層共用的 Model
FPPR_System.Tests       單元測試（業務層）、整合測試（資料層）
DB/                     DDL、Stored Procedure、DML
```

相依方向：展示層 → 業務層 → 資料層，各層皆可參考共用層。

## 資料庫

| 資料表 | 說明 |
|---|---|
| `Users` | 使用者 |
| `Product` | 產品（名稱、價格、手續費率） |
| `LikeList` | 喜好清單（購買數量、扣款帳號、總手續費用、預計扣款總金額），以 `UserID`、`ProductNo` 關聯 |

- 總手續費用 = 價格 × 數量 × 手續費率；預計扣款總金額 = 價格 × 數量 + 總手續費用，皆由 Stored Procedure 計算
- 新增、修改、刪除同時異動 `Product` 與 `LikeList`，以 Transaction 確保一致

## 安全性

- **SQL Injection**：一律透過 Stored Procedure 並以參數傳值
- **XSS**：Razor 輸出自動 HTML 編碼；確認訊息以 `data-*` 屬性傳遞，不嵌入 JavaScript
- **CSRF**：所有 POST 皆驗證 Anti-Forgery Token；刪除僅接受 POST
- **越權**：修改、刪除皆以 `SN` + `UserID` 比對，無法操作他人資料

## 開發

在 Visual Studio 執行時，先啟動資料庫：

```bash
docker compose up -d db db-init
```

執行測試（需資料庫已啟動）：

```bash
dotnet test
```
