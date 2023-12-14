
var tables = Model.Tables;

List<string> pq_list = new List<string>();

foreach( var t in tables)
{
    if(t.Name.StartsWith("P_") || t.Name.StartsWith("L_") || t.Name.StartsWith("M")  )
    {continue;}
    else
    {
        var pq = ExportProperties(t.Partitions, "Expression" );
        var split = pq.Split("\\n");
        
        if(split.Length > 1)
        {
            pq_list.Add(t.Name + "  ||\t\t\t" + split[1] + "\n");
        }
    }
}
Output(pq_list);
