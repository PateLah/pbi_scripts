using System.Text.RegularExpressions;
// ------------------------------------------------- TÄSTÄ YLÖSPÄIN EI TARVITSE TEHDÄ MITÄÄN ------------------------------------------------

// Nimeä uusi kenttäparametritaulu
string name = "P_Upote_kortit";
// Jos et halua oletusryhmittelyä, laita joku oma alla olevaan muuttujaan
string group = "";

// ------------------------------------------------- TÄSTÄ ALASPÄIN EI TARVITSE TEHDÄ MITÄÄN ------------------------------------------------

if(Selected.Columns.Count == 0 && Selected.Measures.Count == 0) throw new Exception("No columns or measures selected!");

// Muodostaa DAX -koodin valituista
//var objects = Selected.Measures.Any() ? Selected.Measures : Selected.Columns.Cast<ITabularTableObject>() ;
string flag = Selected.Measures.Any() ? "Mittarit" : "Sarakkeet" ;
var dax = "";
if(flag == "Mittarit")
{
    var objects = Selected.Measures;
    dax = "{\n    " + string.Join(",\n    ", objects.Select((c,i) => string.Format("(\"{0}\", NAMEOF('{1}'[{2}]), {3}, {4})", ApplyDefaultTranslation(c.Name), c.Table.Name, c.Name, i, group == "" ? "\""+  c.DisplayFolder.Split("\\").Last()+"\"" : "\""+group+"\""))) + "\n}";
}
else
{
    var objects = Selected.Columns.Cast<ITabularTableObject>();
    dax = "{\n    " + string.Join(",\n    ", objects.Select((c,i) => string.Format("(\"{0}\", NAMEOF('{1}'[{2}]), {3}, {4})", ApplyDefaultTranslation(c.Name), c.Table.Name, c.Name, i, group == "" ? "\""+  c.Table.Name+"\"" : "\""+group+"\""))) + "\n}";
}


// Tehdään taulu
var table = Model.AddCalculatedTable(name, dax);

// Tehdään sarakkeet
var te2 = table.Columns.Count == 0;
var nameColumn = te2 ? table.AddCalculatedTableColumn(name, "[Value1]") : table.Columns["Value1"] as CalculatedTableColumn;
var fieldColumn = te2 ? table.AddCalculatedTableColumn(name + " Fields", "[Value2]") : table.Columns["Value2"] as CalculatedTableColumn;
var orderColumn = te2 ? table.AddCalculatedTableColumn(name + " Order", "[Value3]") : table.Columns["Value3"] as CalculatedTableColumn;
var groupColumn = te2 ? table.AddCalculatedTableColumn(name + " Group", "[Value4]") : table.Columns["Value4"] as CalculatedTableColumn;

// Nimetään sarakkeet uudelleen
nameColumn.IsNameInferred = false;
nameColumn.Name = name;
fieldColumn.IsNameInferred = false;
fieldColumn.Name = name + " Fields";
orderColumn.IsNameInferred = false;
orderColumn.Name = name + " Order";
groupColumn.IsNameInferred = false;
groupColumn.Name = name + " Group";

// Parametritaulujen ominaisuudet
nameColumn.SortByColumn = orderColumn;
nameColumn.GroupByColumns.Add(fieldColumn);
fieldColumn.SortByColumn = orderColumn;
fieldColumn.SetExtendedProperty("ParameterMetadata", "{\"version\":3,\"kind\":2}", ExtendedPropertyType.Json);
fieldColumn.IsHidden = true;
orderColumn.IsHidden = true;


// Käännösfunktio
string ApplyDefaultTranslation(string technicalName)
{
    if(true)
    {
        // Etsii mittarinimen lopusta yy[y]-yy[y] tyyppistä kaavaa, jonka muuttaa (yy[y]/yy[y])
        string pattern = @"_(\w{2,3})-(\w{2,3}$)";        

        // Sulutetaan loppuliitteitä
        technicalName = Regex.Replace(technicalName, pattern, " ($1/$2)");
        // Plussat
        technicalName = technicalName.Replace("plus", "+");
        // Välilyönnit
        technicalName = technicalName.Replace("_", " ");
        technicalName = technicalName.Replace("-", " ");
        // Taulujen etuliite
        technicalName = Regex.Replace(technicalName, @"^([A-Z])\s", "$1_");
        // Prosentit
        technicalName = Regex.Replace(technicalName, @"(\w+)prosentti", "$1-%");
        technicalName = Regex.Replace(technicalName, @"prosentti(\w+)", "%-$1");
        // Summa
        technicalName = technicalName.Replace(" summa", "");
        // Organisaation ääkköset
        string aoReplaced = technicalName.Replace("yksikko", "yksikkö");
        aoReplaced = aoReplaced.Replace("ryhma", "ryhmä");
        aoReplaced = aoReplaced.Replace("iinteisto", "iinteistö");  // Kiinteistö/kiintestö
        aoReplaced = aoReplaced.Replace("aynti", "äynti");          // Käynti/käynti
        aoReplaced = aoReplaced.Replace("Ika", "Ikä");
        aoReplaced = aoReplaced.Replace("aytetty", "äytetty");      // Käytetty/käytetty
        aoReplaced = aoReplaced.Replace("asittely", "äsittely");    // Käsittely
        aoReplaced = aoReplaced.Replace("aytto", "äyttö");
        aoReplaced = aoReplaced.Replace("Pyha", "Pyhä");
        aoReplaced = aoReplaced.Replace("Paiva", "Päivä");
        aoReplaced = aoReplaced.Replace("paiva", "päivä");
        aoReplaced = aoReplaced.Replace("pitka", "pitkä");
        aoReplaced = aoReplaced.Replace("neljannes", "neljännes");
        
        string translatedName = aoReplaced;
        return translatedName;
        
    }
}