namespace ProjektClicker
{
    public class ClickyFactService
{
    private readonly Random _rand = new();
    private readonly string[] _facts =
    [
        "Aligatory żyją na ziemi od 37 milionów lat.",
        "Osły mogą rżeć ale tylko jeśli mogą podnieść ogon.",
        "Zapalniczka została wynaleziona 3 lata przed zapałkami.",
        "W Finlandii jest najwięcej zespołów heavy metalowych per capita.",
        "Ważki mimo posiadania 6 odnóży nie potrafią chodzić.",
        "Banan jest klasyfikowany jako jagoda.",
        "Papież Jan Paweł II został mianowany honorowym członkiem drużyny Harlem Globetrotters.",
        "Na każdym kontynencie, z wyjątkiem Antarktydy, znajduje się miasto o nazwie „Rzym”.",
        "Królowa Elżbieta II była mechanikiem z wykształcenia."
    ];
    public string GetRandomFact() => _facts[_rand.Next(_facts.Length)];
}
}
