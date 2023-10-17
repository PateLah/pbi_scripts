using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

//-------- Jos haluat muuttaa/poistaa jotain tietojoukolle spesifistä ---------------

string changeFrom1 = " summa";
string changeTo1 = "";

//-------- Toinen mahdollisuus muuttaa/poistaa jotain tietojoukolle spesifistä ------

string changeFrom2 = "";
string changeTo2 = "";

//-----------------------------------------------------------------------------------

// Lisää käännökset kaikkiin näkyvillä oleviin käännettäviin objekteihin kaikille kielille
foreach(var culture in Model.Cultures)
{
    ApplyDefaultTranslation(Model, culture);
    foreach(var perspective in Model.Perspectives)
        ApplyDefaultTranslation(perspective, culture);
    foreach(var table in Model.Tables.Where(t => t.IsVisible))
        ApplyDefaultTranslation(table, culture);
    foreach(var measure in Model.AllMeasures.Where(m => m.IsVisible))
        ApplyDefaultTranslation(measure, culture);
    foreach(var column in Model.AllColumns.Where(c => c.IsVisible))
        ApplyDefaultTranslation(column, culture);
    foreach(var hierarchy in Model.AllHierarchies.Where(h => h.IsVisible))
        ApplyDefaultTranslation(hierarchy, culture);
    foreach(var level in Model.AllLevels.Where(l => l.Hierarchy.IsVisible))
        ApplyDefaultTranslation(level, culture);
}

void ApplyDefaultTranslation(ITranslatableObject obj, Culture culture)
{
    // Lisää käännöksen vain, jos sitä ei vielä ole olemassa
    if(string.IsNullOrEmpty(obj.TranslatedNames[culture]))
    {
        // Etsii mittarinimen lopusta yy[y]-yy[y] tyyppistä kaavaa, jonka muuttaa (yy[y]/yy[y])
        string pattern = @"_(\w{2,3})-(\w{2,3}$)";        

        string technicalName = obj.Name;
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

        // Omat muutokset
        if (changeFrom1 != "")
        {
            translatedName = aoReplaced.Replace(changeFrom1, changeTo1);
        }
        if (changeFrom2 != "")
        {
            translatedName = aoReplaced.Replace(changeFrom2, changeTo2);
        }
        
        obj.TranslatedNames[culture] = translatedName;
        

        // Lisää selitteiden käännökset - ei tee muutoksia
        var dObj = obj as IDescriptionObject;
        if(dObj != null && string.IsNullOrEmpty(obj.TranslatedDescriptions[culture])
            && !string.IsNullOrEmpty(dObj.Description))
        {
            obj.TranslatedDescriptions[culture] = dObj.Description;
        }

        // Lisää kansioiden käännökset - ei tee muutoksia
        var fObj = obj as IFolderObject;
        if(fObj != null && string.IsNullOrEmpty(fObj.TranslatedDisplayFolders[culture])
            && !string.IsNullOrEmpty(fObj.DisplayFolder))
        {
            fObj.TranslatedDisplayFolders[culture] = fObj.DisplayFolder;
        }
    }
}
