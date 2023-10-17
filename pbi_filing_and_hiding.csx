using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;


var _all_tables = Model.Tables;


foreach(var table in _all_tables)
{
//  Piilotetaan fakta-, parametri- ja metatietotaulut
    var t_n = table.Name;
    string[] tableNameParts = t_n.Split('_');
    string tablePrefix = tableNameParts[0];
    if (tablePrefix == "F" || tablePrefix == "M" || tablePrefix == "P")
    {
        table.IsHidden = true;
    }

    var cols = table.Columns.ToList();

    foreach(var c in cols)
    {
//      Piilotetaan kaikista tauluista Dim_ -sarakkeet
        var c_n = c.Name;
        string[] parts = c_n.Split('_');
        string groupName = parts[0];
        if (groupName == "Dim")
        {
            c.IsHidden = true;
        }
    }
}

var _org_tables = Model.Tables.Where(t => t.Name.StartsWith("L_Organisaatio") || t.Name.StartsWith("D_Organisaatio")).ToList(); // <-- toimii
//Output(_org_tables);


foreach(var table in _org_tables)
{
/*
    TÄSSÄ TEHDÄÄN HIERARKIA, MUTTA SIIHEN EI PYSTY LISÄTÄ TASOJA, JOTEN TURHA LISÄTÄ
    var t_n = table.Name;
    string[] tableNameParts = t_n.Split('_');
    string hierarchyName = tableNameParts[2];
    // jos moniosainen nimi yhdistetään loppuosat takaisin yhteen:
    if (tableNameParts.Length > 2)
    {
        hierarchyName = string.Join(" ", tableNameParts.Skip(2));
    }
    // ensimäinen kirjain isolla ja hierarkia loppuun
    hierarchyName = char.ToUpper(hierarchyName[0]) + hierarchyName.Substring(1) + " hierarkia";  
    // hierarkia tauluun
    if ( table.Hierarchies.Contains( hierarchyName ))
    {
        Output("Hierarchy already exists");
    }
    else
    {
    var newHierarchy = table.AddHierarchy( hierarchyName );
    }
*/
    
    // Sarakkeiden kansioiminen
    var cols = table.Columns.ToList();

    foreach(var c in cols)
    {
        var c_n = c.Name;
        string[] parts = c_n.Split('_');
        string groupName = parts[0];
        if (groupName == "Dim")
        {
            c.IsHidden = true;
        }
        else
        {
        Model.Tables[table.Name].Columns[c.Name].DisplayFolder = groupName;
        }
        
    }
}
