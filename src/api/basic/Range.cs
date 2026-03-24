using Newtonsoft.Json;

namespace ChatBot.api.basic;

public class Range {
    [JsonProperty("start")]
    private int _start;
    public int Start {
        get => HasStart ? _start : 0;
        set {
            if (HasStart)
                _start = value;
        }
    }

    [JsonProperty("end")]
    private int _end;
    public int End {
        get => HasEnd ? _end : 0;
        set {
            if (HasEnd)
                _end = value;
        }
    }

    [JsonProperty("has_start")]
    public bool HasEnd { get; private set; }
    
    [JsonProperty("has_end")]
    public bool HasStart { get; private set; }
    
    [JsonIgnore]
    public int Length => Math.Abs(End - Start);
    

    public Range() {
        _start = int.MinValue;
        _end = int.MaxValue;
        HasStart = false;
        HasEnd = false;
    }
    
    public Range(int start, int end) {
        _start = start;
        _end = end;
        HasStart = true;
        HasEnd = true;
    }
    
    [JsonConstructor]
    public Range(
        [JsonProperty("start")] int start,
        [JsonProperty("end")] int end,
        [JsonProperty("has_start")] bool hasStart,
        [JsonProperty("has_end")] bool hasEnd) {
        _start = start;
        _end = end;
        HasStart = hasStart;
        HasEnd = hasEnd;
    }

    public void Order() {
        if (End < Start) {
            (Start, End) = (End, Start);
        }
    }

    public void AdjustTo(int start, int end) {
        var exceed = End - end;
        if (exceed < 0) exceed = 0;
        End -= exceed;
        Start -= exceed;

        exceed = start - Start;
        if (exceed < 0) exceed = 0;
        
        Start += exceed;
    }

    public void MoveStartIfLess(int value) {
        var exceed = Start - value;
        if (exceed >= 0)
            return;
        Start -= exceed;
    }
    
    public void MoveEndIfGreater(int value) {
        var lose = End - value;
        if (lose <= 0)
            return;
        End -= lose;
    }
    
    public void CutLengthTo(int length) {
        var exceed = Length - length;
        if (exceed <= 0) return;
        
        Start -= exceed;
        End -= exceed;
    }
    
    public void SetStart(int val) {
        HasStart = true;
        Start = val;
    }
    
    public void SetEnd(int val) {
        HasEnd = true;
        End = val;
    }
    
    public void UnSetStart() {
        Start = int.MinValue;
        HasStart = false;
    }
    
    public void UnSetEnd() {
        End = int.MaxValue;
        HasEnd = false;
    }
}