# ASP.NET Booklog — 読書ログ（ASP.NET Core + EF Core + PostgreSQL + Docker + Render）

本の **タイトル / 著者 / メモ / ステータス（Unread/Reading/Done）** を管理する読書ログアプリです。  
**ASP.NET Core 8（Razor Pages） + EF Core + PostgreSQL**

ローカル（Docker）で実行する方法と、公開URLから試す方法を記載します。

---

## 機能
- 本の登録・一覧表示（新しい順）
- ステータスでフィルタ（All / Unread / Reading / Done）
- Note を UI でトリム表示
- 起動時に **自動マイグレーション**（テーブル未作成でも OK）

---

## 技術スタック
- **Framework**: ASP.NET Core 8 (Razor Pages)
- **ORM**: Entity Framework Core + Npgsql
- **DB**: PostgreSQL
- **Infra**: Docker / Render.com
- **Lang**: C#

---

## プロジェクト構成（抜粋）
aspnet-booklog/  
├─ src/  
│ ├─ aspnet-booklog.csproj  
│ ├─ Program.cs  
│ ├─ Data/AppDbContext.cs  
│ ├─ Models/Book.cs  
│ ├─ Pages/  
│ │ ├─ Index.cshtml.cs  
│ │ ├─ Index.cshtml  
│ │ └─ Shared/_Layout.cshtml  
│ └─ Migrations/ # ← ef migrations 生成物（コミット推奨）  
├─ Dockerfile  
└─ .dockerignore  

---

## 依存パッケージ（NuGet）
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Npgsql`
- `Microsoft.EntityFrameworkCore.Design`（マイグレーション用）

> ローカルに .NET SDK を入れなくても、**Docker 経由**で `dotnet add package` / `dotnet ef` を実行できます。

---

## 環境変数
- `DATABASE_URL`（例：`postgresql://USER:PASSWORD@HOST:PORT/DBNAME`）  
  ※ Render の接続文字列が `postgres://` でもアプリ側で安全に処理します。

---

## ローカルで実行する方法（Docker / WSL）

### 前提
以下がインストール・設定済みであることを前提とします。

- WSL（Windows Subsystem for Linux）
- Docker Desktop
- WSL と Docker の連携設定（WSL integration）が有効

---

### 1. Git をインストール（未インストールの場合）

```bash
sudo apt update
sudo apt install -y git
```

---

### 2. リポジトリをクローン

#### Public リポジトリ（HTTPS）
```bash
git clone https://github.com/ynakao55/aspnet-booklog.git
cd aspnet-booklog
```

---

### 3. 起動（Docker Compose）

#### 通常の起動 / 停止
```bash
docker compose up -d --build
docker compose down
```

#### 確実に作り直して起動したい場合（推奨）
コンテナ・ボリューム・不要な関連コンテナも整理してから再作成します。

```bash
docker compose down -v --remove-orphans
docker compose up -d --build --force-recreate
```

---

### 4. ブラウザで開く

起動後、以下のURLをブラウザで開いてください。

- http://localhost:8080

---

## 公開URLから試す方法（Web）

以下のURLからアクセスできます。

- https://aspnet-booklog.ysnko.com

### 初回アクセス時の流れ
1. 上記リンクを開く
2. ログイン方法で **メール（PINコード）** を選択
3. メールアドレスを入力
4. 届いたメールに記載された **PINコード** を確認
5. PINコードを入力してログイン

### なぜPINが必要？
このサイトは Cloudflare Access により保護されており、  
**本人確認のためにワンタイムPIN（使い捨てコード）** を使用しています。  
パスワードを作成・管理しなくても、メールで簡単に本人確認できます。

---

## ローカル実行時の補足（トラブルシュート）

### 起動しているか確認
```bash
docker compose ps
```

### ログを確認
```bash
docker compose logs -f
```

### 画面が開かない / 反映されない場合
```bash
docker compose down -v --remove-orphans
docker compose up -d --build --force-recreate
```

---

## ライセンス

MIT License

---
