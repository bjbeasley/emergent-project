using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CharacterName
{
    public CharacterName (string first, string surname = "", string nickname = "", string title = "")
    {
        First = first;
        Title = title;
        Surname = surname;
        Nickname = nickname;
    }

    public string First { get; set; }

    public string Title { get; set; }

    public string Surname { get; set; }

    public string Nickname { get; set; }

    public override string ToString ()
    {
        return GetFull();
    }

    public string GetFull ()
    {
        StringBuilder sb = new StringBuilder();
        if(!string.IsNullOrEmpty(Title))
        {
            sb.Append(Title);
            sb.Append(" ");
        }
        sb.Append(First);
        if(!string.IsNullOrEmpty(Nickname))
        {
            sb.Append(" \"");
            sb.Append(Nickname);
            sb.Append("\" ");
        }
        if(!string.IsNullOrEmpty(Surname))
        {
            //sb.Append(" ");
            //sb.Append(Surname);
        }

        return sb.ToString();
    }
}

