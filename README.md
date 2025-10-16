ASP.NET Booklog — 読書ログ（ASP.NET Core + EF Core + PostgreSQL + Docker + Render）

本の タイトル / 著者 / メモ / ステータス（Unread/Reading/Done） を管理する読書ログアプリです。
ASP.NET Core 8（Razor Pages） + EF Core + PostgreSQL。Docker で動かし、Render.com にそのままデプロイできます。

機能

本の登録・一覧表示（新しい順）

ステータスでフィルタ（All / Unread / Reading / Done）

Note の長文はトリム表示（UI で省略）

起動時に 自動マイグレーション（テーブル未作成でも OK）

技術スタック

Framework: ASP.NET Core 8 (Razor Pages)

ORM: Entity Framework Core + Npgsql

DB: PostgreSQL

Infra: Docker / Render.com

Lang: C#

プロジェクト構成（抜粋）
aspnet-booklog/
├─ src/
│  ├─ aspnet-booklog.csproj
│  ├─ Program.cs
│  ├─ Data/AppDbContext.cs
│  ├─ Models/Book.cs
│  ├─ Pages/
│  │  ├─ Index.cshtml.cs
│  │  ├─ Index.cshtml
│  │  └─ Shared/_Layout.cshtml
│  └─ Migrations/              # ← ef migrations 生成物（コミット推奨）
├─ Dockerfile
└─ .dockerignore

依存パッケージ（NuGet）

Npgsql.EntityFrameworkCore.PostgreSQL

Npgsql

Microsoft.EntityFrameworkCore.Design（マイグレーション用）

SDK をローカルに入れず、Docker 経由で dotnet add package / dotnet ef を実行できます。

環境変数

DATABASE_URL（例：postgresql://USER:PASSWORD@HOST:PORT/DBNAME）
※ Render の接続文字列が postgres:// の場合でもアプリ内で安全に処理されます。

ローカル実行（Docker）

マイグレーション作成（未作成の場合）

cd aspnet-booklog

# ツール＆マイグレーション
docker run --rm -v "$PWD/src:/src" -w /src mcr.microsoft.com/dotnet/sdk:8.0 bash -lc '
  set -e
  test -f /src/.config/dotnet-tools.json || dotnet new tool-manifest
  dotnet tool install dotnet-ef || true
  dotnet tool restore
  dotnet ef migrations add InitialCreate --output-dir Migrations
'


ビルド & 起動

docker build -t aspnet-booklog .
docker run --rm -p 8080:8080 \
  -e DATABASE_URL="postgresql://<user>:<pass>@<host>:<port>/<db>" \
  aspnet-booklog
# http://localhost:8080

Render へのデプロイ手順（Docker）

GitHub にこのリポジトリを push

Render ダッシュボード → New + → Web Service → GitHub リポジトリを選択

Environment に DATABASE_URL を設定（Render の Postgres の接続文字列）

ビルドは Dockerfile 自動検出、Start は既定の ENTRYPOINT を利用

初回起動時に 自動マイグレーション が走ります

無料プラン（Pre-Deploy コマンド無し）でも動作する設計です。
ログに localhost:5432 が見えたら DATABASE_URL 未設定/誤りを疑ってください。

主要エンドポイント

/：一覧 + フィルタ（All / Unread / Reading / Done）

ライセンス

MIT License（ご自由にご利用ください）。