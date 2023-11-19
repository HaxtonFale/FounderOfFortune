namespace FounderOfFortune.Game

module Say =
    type Suit =
        | Cups      = 0
        | Wands     = 1
        | Swords    = 2
        | Pentacles = 3


    type Card =
        | Minor of Suit * int
        | Major of int

        static member (+) (card: Card, amount: int): Card =
            match card with
            | Major value -> Major (value + amount)
            | Minor (suit, value) -> Minor (suit, value + amount)

    type MinorStack = { Suit: Suit; Top: int }
    let topCard (stack: MinorStack): Card = Minor (stack.Suit, stack.Top)
    let promoteMinor (stack: MinorStack) (card: Card): MinorStack =
        let makeStack (Minor (s, v)): MinorStack = {Suit = s; Top = v}
        match card with
        | Major _ -> invalidArg (nameof(card)) "Cannot promote major arcana onto a minor arcana stack."
        | Minor _ -> if ((topCard stack) + 1 = card) then makeStack card else invalidOp "Cannot promote card"

    type MajorStack = { Left: Option<int>; Right: Option<int> }

    let adjacent (left: Card) (right: Card): bool =
        match (left, right) with
        | (Minor (ls, lv), Minor (rs, rv)) -> if (ls = rs) then (abs (lv - rv) = 1) else false
        | (Major lv, Major rv) -> abs (lv - rv) = 1
        | _ -> false

    let hello name =
        printfn "Hello %s" name
