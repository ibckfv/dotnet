
string? ReverseWord(string word)
{
    return new string(word.ToCharArray().Reverse().ToArray());
}

string? ReverseSentence(string sentence)
{
    var split = sentence.Split(" ")
        .Select(i =>
            i = new string(i.ToCharArray().Reverse().ToArray()));
    
    /*for (int i = 0; i < split.Length; i++)
    {
        split[i] = new string(split[i].ToCharArray().Reverse().ToArray());
    }*/

    return string.Join(" ", split);
}

Console.WriteLine(ReverseWord("Привет"));
Console.WriteLine(ReverseSentence("Привет , мир"));