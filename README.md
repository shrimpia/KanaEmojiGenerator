﻿# かな絵文字ジェネレーター for Misskey

![こんなのが作れるよ](https://media.shrimpia.network/mk-shrimpia/files/0fe77eb9-9af7-477e-9cdc-cb3d01beaa5e.png)

Misskey向けのひらがな絵文字データおよび、インポート用のjsonファイルを生成するツールです。

## 使い方

1. 実行ファイル（Windowsの場合は KanaEmojiGenerator.exe）を実行します。
2. output ディレクトリに結果が生成されるので、中身ごとzipにします
3. zipをMisskeyのカスタム絵文字管理画面でインポートします

## カスタマイズ
生成される画像をカスタマイズできます。

### フォントのカスタマイズ
同梱されている `font.ttf` をお好きなフォントに変更するだけで、使用するフォントを変更できます。形式はTrueTypeである必要があります。
なお、本ツールは等幅フォントを前提としています。特殊なフォントを利用すると生成データが狂う可能性があります。

### 生成される画像のカスタマイズ
色やサイズを調整できます。
フォントによっては、既存の設定と相性が悪い可能性があるので、フォントサイズを調整してみてください。

## ライセンス

ソフトウェアのソースコードは [MITライセンス](LICENSE) です。

フォントには瀬戸フォントを用いています。瀬戸フォントはSIL Open Font License 1.1でライセンスされています。
