using System;

namespace FlashOWare.Tool.Core;

public class ShakespeareStore
{
    private readonly Play[] plays = new Play[]
    {
        new Play("Hamlet"),
        new Play("Titus Andronicus"),
        new Play("King Lear"),
        new Play("Othello"),
        new Play("Romeo and Juliet"),
    };

    public bool IsAvailable(string play)
    {
        for(int i = 0; i < plays.Length; i++)
        {
            Play p = plays[i];
            if(play == p.Name)
            {
                return p.IsAvailable;
            }
        }
        return false;
    }

    public void CheckOut(string play)
    {
        for(int i = 0; i < plays.Length; i++)
        {
            Play p = plays[i];
            if(play == p.Name)
            {
                if(p.IsAvailable)
                {
                    p.IsAvailable = false;
                    return;
                }
                throw new PlayNotAvailableException(play);
            }
        }
    }

    public void CheckIn(string play)
    {
        for(int i = 0; i < plays.Length; i++)
        {
            Play p = plays[i];
            if(play == p.Name)
            {
                if(!p.IsAvailable)
                {
                    p.IsAvailable = true;
                    return;
                }
                throw new PlayIsAvailableException(play);
            }
        }
    }
}

internal sealed class Play
{
    public string Name { get; }
    public bool IsAvailable { get; set; }

    public Play(string name)
    {
        Name = name;
        IsAvailable = true;
    }
}

public sealed class PlayNotAvailableException : Exception
{
    public PlayNotAvailableException(string play)
    {
        PlayName = play;
    }

    public string PlayName { get; }
}

public sealed class PlayIsAvailableException : Exception
{
    public PlayIsAvailableException(string play)
    {
        PlayName = play;
    }

    public string PlayName { get; }
}
