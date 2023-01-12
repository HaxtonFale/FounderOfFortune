using FounderOfFortune.Game.Model;

namespace FounderOfFortune.Game.Collections;

public class MajorArcanaStacks {
    private readonly MajorArcana? _left;
    private readonly MajorArcana? _right;

    public MajorArcanaStacks() {
        _left = null;
        _right = null;
    }

    private MajorArcanaStacks(MajorArcana? left, MajorArcana? right) {
        _left = left;
        _right = right;
    }

    public MajorArcanaStacks Ascend(MajorArcana card) {
        var success = false;
        var newLeft = _left;
        if ((_left == null && card.Value == 0) || (_left != null && card.Value == _left.Value.Value + 1)) {
            newLeft = card;
            success = true;
        }

        var newRight = _right;
        if ((_right == null && card.Value == 21) || (_right != null && card.Value == _right.Value.Value + 1)) {
            newRight = card;
            success = true;
        }

        if (success) return new MajorArcanaStacks(newLeft, newRight);
        throw new ArgumentException("Card value ineligible for ascension", nameof(card));
    }

    public Tuple<MajorArcana?, MajorArcana?> TopCards => new(_left, _right);
}