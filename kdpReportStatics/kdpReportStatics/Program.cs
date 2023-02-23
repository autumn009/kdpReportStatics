using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

if ( args.Length != 3) {
    Console.WriteLine("usage: kdpReportStatics FILE_NAME1 FILE_NAME2 OUTPUT_FILE_NAME");
    Console.WriteLine("All File must be CSV"  );
    Console.WriteLine("FILE_NAME1: 販売数合算 tab");
    Console.WriteLine("FILE_NAME2: 既読KENP tab");
    return;
}

string src1FileName = args[0];
string src2FileName = args[1];
string dstFileName = args[2];

var dic = new Dictionary<string, OutputRecord>();

using (var reader = new StreamReader(src1FileName))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {
    var records = csv.GetRecords<販売数合算>();
    foreach (var record in records) {
        if (!dic.ContainsKey(record.タイトル)) {
            var nr = new OutputRecord();
            nr.FirstDate = dateFixer(record.ロイヤリティ発生日);
            nr.Name = record.タイトル;
            dic.Add(record.タイトル, nr);
        }
        dic[record.タイトル].PricedCountSum += record.実質販売数;
        updateDate(dic[record.タイトル], record.ロイヤリティ発生日);
    }
}

using (var writer = new StreamWriter(dstFileName)) {
    writer.Write((char)0xfeff);
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture)) {
        csv.WriteRecords(dic.Values.OrderBy(c => c.FirstDate).ToArray());
    }
}

ProcessStartInfo pi = new ProcessStartInfo() {
    FileName = dstFileName,
    UseShellExecute = true,
};
Process.Start(pi);
Console.WriteLine("Done");

void updateDate(OutputRecord rec, string date) {
    if (rec.FirstDate == null) rec.FirstDate = dateFixer(date);
    else {
        if (string.Compare(rec.FirstDate, date)>0) rec.FirstDate = dateFixer(date);
    }
}

string dateFixer(string s) {
    var date = DateTime.ParseExact(s, "yyyy-MM", null);
    return date.ToString("yyyy/MM/dd");
}


public class 販売数合算 {
    public string? ロイヤリティ発生日 { get; set; }
    public string? タイトル { get; set; }
    public string? 著者名 { get; set; }
    [Name("ASIN/ISBN")]
    public string? ASIN { get; set; }
    public string? マーケットプレイス { get; set; }
    public string? ロイヤリティの種類 { get; set; }
    public string? 取引の種類 { get; set; }
    public int 販売数量 { get; set; }
    public string? 払い戻し数 { get; set; }
    public int 実質販売数 { get; set; }
    [Name("平均希望小売価格 (税別)")]
    public string? 平均希望小売価格 { get; set; }
    [Name("平均販売価格 (税別)")]
    public string? 平均販売価格 { get; set; }
    [Name("平均配信/制作コスト")]
    public string? 平均配信 { get; set; }
    public string? ロイヤリティ { get; set; }
    public string? 通貨 { get; set; }
}

#if false
public class 電子書籍の注文数 {
    public string? 日付 { get; set; }
    public string? タイトル { get; set; }
    public string? 著者名 { get; set; }
    public string? ASIN { get; set; }
    public string? マーケットプレイス { get; set; }
    public int 有料配布数 { get; set; }
    public int 無料配布数 { get; set; }
}
#endif

public class 既読KENP
{
    public string? Date { get; set; }
    public string? Name { get; set; }
    public string? Author { get; set; }
    public string? ASIN { get; set; }
    public string? Market { get; set; }
    public int KENP { get; set; }
}

public class OutputRecord {
    public string? FirstDate { get; set; }
    public string? Name { get; set; }
    public int KENPSum { get; set; }
    public int PricedCountSum { get; set; }
    public int NonPricedCountSum { get; set; }
}
