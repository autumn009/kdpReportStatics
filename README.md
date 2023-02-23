# kdpReportStatics

KDPからダウンロードされた売上げデータのExcelファイルの販売数合算と既読KENPのタブをCSV保存したファイルを読み込んで簡単な集計を行います。

ダウンロードしたExcelファイルは月別データになっているので、これを合計した値を算出します。
出力はCSVです。

　C#で書かれたコンソールアプリで書式は以下の通りです。

usage: kdpReportStatics FILE_NAME1 FILE_NAME2 OUTPUT_FILE_NAME

FILE_NAME1: 販売数合算

FILE_NAME2: 既読KENP
