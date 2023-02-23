using CsvHelper;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

if ( args.Length != 3) {
    Console.WriteLine("usage: kdpReportStatics FILE_NAME1 FILE_NAME2 OUTPUT_FILE_NAME");
    Console.WriteLine("All File must be CSV"  );
    Console.WriteLine("FILE_NAME1: 電子書籍の注文数 tab");
    Console.WriteLine("FILE_NAME2: 既読KENP tab");
    return;
}

string src1FileName = args[0];
string src2FileName = args[1];
string dstFileName = args[2];

var dic = new Dictionary<string, OutputRecord>();

using (var reader = new StreamReader(src1FileName))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture)) {
    var records = csv.GetRecords<電子書籍の注文数>();
    foreach (var record in records) {
        if (!dic.ContainsKey(record.タイトル)) {
            var nr = new OutputRecord();
            nr.FirstDate = record.日付;
            nr.Name = record.タイトル;
            dic.Add(record.タイトル, nr);
        }
        dic[record.タイトル].PricedCountSum += record.有料配布数;
        dic[record.タイトル].NonPricedCountSum += record.無料配布数;
        updateDate(dic[record.タイトル], record.日付);
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
    var date = DateTime.ParseExact(s, "yyyy-MM-dd", null);
    return date.ToString("yyyy/MM/dd");
}

public class 電子書籍の注文数 {
    public string? 日付 { get; set; }
    public string? タイトル { get; set; }
    public string? 著者名 { get; set; }
    public string? ASIN { get; set; }
    public string? マーケットプレイス { get; set; }
    public int 有料配布数 { get; set; }
    public int 無料配布数 { get; set; }
}

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
