# Mögliche Test-Grammatiken

Basierend auf den Konzepten von Taschenrechner und RobotProgramming.

---

## Typ 1: Ausdrücke mit Operatoren (ähnlich Taschenrechner)

### 1.1 Logische Ausdrücke

**ABNF:**
```abnf
expression ::= term { "OR" term }
term ::= factor { "AND" factor }
factor ::= ["NOT"] atom
atom ::= "TRUE" | "FALSE" | variable | "(" expression ")"
variable ::= letter { letter | digit }
```

**Beispiele:**
```
TRUE AND NOT FALSE OR x
(x OR y) AND NOT z
TRUE AND TRUE OR FALSE
```

**Konzepte:**
- Operator-Präzedenz: NOT > AND > OR
- Links-Assoziativität bei AND/OR
- Klammer-Ausdrücke

**Parser-Struktur:**
- `ParseExpression()` für OR (while-loop)
- `ParseTerm()` für AND (while-loop)
- `ParseFactor()` für NOT (if)
- `ParseAtom()` für TRUE/FALSE/Variable/Klammern

**Code-Beispiel:**
```csharp
// Ebene 1: OR (niedrigste Priorität)
IExpression ParseExpression() {
    IExpression left = ParseTerm();
    while (Match("OR")) {
        Consume();
        IExpression right = ParseTerm();
        left = new OrNode(left, right);
    }
    return left;
}

// Ebene 2: AND
IExpression ParseTerm() {
    IExpression left = ParseFactor();
    while (Match("AND")) {
        Consume();
        IExpression right = ParseFactor();
        left = new AndNode(left, right);
    }
    return left;
}

// Ebene 3: NOT
IExpression ParseFactor() {
    if (Match("NOT")) {
        Consume();
        IExpression operand = ParseFactor();  // Rekursion für NOT NOT ...
        return new NotNode(operand);
    }
    return ParseAtom();
}

// Ebene 4: Basiselemente
IExpression ParseAtom() {
    if (Match("TRUE")) { Consume(); return new TrueNode(); }
    if (Match("FALSE")) { Consume(); return new FalseNode(); }
    if (Match("(")) {
        Consume();
        IExpression expr = ParseExpression();
        Consume();  // )
        return expr;
    }
    // Variable...
}
```

**Beispiel-Parsing:** `TRUE OR FALSE AND NOT TRUE`
→ Geparst als: `TRUE OR (FALSE AND (NOT TRUE))`
→ Ergebnis: `TRUE OR (FALSE AND FALSE)` = `TRUE OR FALSE` = `TRUE`

---

### 1.2 Taschenrechner mit Modulo und Fakultät

**ABNF:**
```abnf
expression ::= term { ("+" | "-") term }
term ::= power { ("*" | "/" | "%") power }
power ::= factor ["^" power]
factor ::= number ["!"] | variable | "(" expression ")"
number ::= digit { digit } [ "," digit { digit } ]
variable ::= letter { letter | digit }
```

**Beispiele:**
```
5! % 10 + 2^3
3 + 4! * 2
100 % 7 - 1
```

**Konzepte:**
- Modulo `%` auf gleicher Ebene wie `*` und `/`
- Fakultät `!` als **Postfix-Operator** (nach der Zahl)
- Rechts-Assoziativität bei `^`

**Besonderheit:**
```csharp
// In ParseFactor():
if (Match(TokenTypes.Number)) {
    double num = ParseNumber();
    if (Match(TokenTypes.Factorial)) {  // !
        Consume();
        return new FactorialNode(new NumberNode(num));
    }
    return new NumberNode(num);
}
```

---

### 1.3 Vergleiche und Bedingungen

**ABNF:**
```abnf
expression ::= orTerm { "OR" orTerm }
orTerm ::= andTerm { "AND" andTerm }
andTerm ::= comparison
comparison ::= term [ ("<" | ">" | "==" | "!=") term ]
term ::= factor { ("+" | "-") factor }
factor ::= number | variable | "(" expression ")"
```

**Beispiele:**
```
x + 5 > 10 AND y == 3
a != b OR c < 5
(x + y) == 10 AND z > 0
```

**Konzepte:**
- Mehrere Präzedenz-Ebenen: Logik > Vergleich > Arithmetik
- Boolean-Rückgabewerte bei Vergleichen

**Parser-Struktur:**
```
ParseExpression()     → OR
  ParseOrTerm()       → AND
    ParseAndTerm()    → delegiert zu ParseComparison()
      ParseComparison() → <, >, ==, !=
        ParseTerm()   → +, -
          ParseFactor() → Zahlen, Variablen, Klammern
```

---

## Typ 2: Statement-basiert (ähnlich RobotProgramming)

### 2.1 Turtle Graphics

**ABNF:**
```abnf
program ::= { statement }
statement ::= "FORWARD" number
            | "BACK" number
            | "LEFT" number
            | "RIGHT" number
            | "PENUP"
            | "PENDOWN"
            | "REPEAT" number "{" { statement } "}"
number ::= digit { digit }
```

**Beispiele:**
```
FORWARD 100
RIGHT 90
REPEAT 4 {
    FORWARD 100
    RIGHT 90
}
PENUP
FORWARD 50
PENDOWN
```

**Konzepte:**
- Verschachtelte REPEAT-Blöcke
- Befehle mit/ohne Parameter
- State (Pen up/down)

**Execute-Logik:**
```csharp
public class ForwardExpression : IExpression {
    public int Distance { get; set; }

    public void Execute(TurtleContext ctx) {
        ctx.X += Math.Cos(ctx.Angle) * Distance;
        ctx.Y += Math.Sin(ctx.Angle) * Distance;
        if (ctx.PenDown) ctx.DrawLine(...);
    }
}
```

---

### 2.2 Textverarbeitung

**ABNF:**
```abnf
program ::= { statement }
statement ::= "PRINT" string
            | "REPLACE" string "WITH" string
            | "APPEND" string
            | "CLEAR"
            | "IF" condition "{" { statement } "}"
            | "REPEAT" number "{" { statement } "}"

condition ::= "CONTAINS" string
            | "STARTSWITH" string
            | "ENDSWITH" string
            | "LENGTH" (">" | "<" | "==") number

string ::= '"' { any_char } '"'
```

**Beispiele:**
```
PRINT "Hello World"
APPEND " - Ende"
IF CONTAINS "error" {
    REPLACE "error" WITH "warning"
    PRINT "Fixed!"
}
REPEAT 3 {
    APPEND "!"
}
```

**Konzepte:**
- IF-Bedingungen ohne ELSE
- String-Parameter mit Anführungszeichen
- Textzustand (Buffer)

**Condition-Parsing:**
```csharp
public class IfExpression : IExpression {
    public ICondition Condition { get; set; }
    public List<IExpression> Body { get; set; }

    public void Execute(TextContext ctx) {
        if (Condition.Evaluate(ctx)) {
            foreach (var stmt in Body) {
                stmt.Execute(ctx);
            }
        }
    }
}
```

---

### 2.3 Spielcharakter-Steuerung

**ABNF:**
```abnf
program ::= { statement }
statement ::= "MOVE" direction number
            | "ATTACK"
            | "DEFEND"
            | "COLLECT" item
            | "USE" item
            | "REPEAT" number "{" { statement } "}"
            | "WHILE" condition "{" { statement } "}"

direction ::= "NORTH" | "SOUTH" | "EAST" | "WEST"
item ::= "SWORD" | "POTION" | "KEY" | "SHIELD"
condition ::= "ENEMY_NEAR"
            | "HEALTH_LOW"
            | "HAS" item
            | "AT_DOOR"
```

**Beispiele:**
```
MOVE NORTH 5
COLLECT KEY
WHILE ENEMY_NEAR {
    ATTACK
    IF HEALTH_LOW {
        USE POTION
    }
}
MOVE EAST 3
USE KEY
```

**Konzepte:**
- WHILE-Schleifen (schwieriger!)
- Enumerations (Direction, Item)
- Verschachtelte IF in WHILE
- Zustandsabfragen (Bedingungen)

**WHILE-Implementierung:**
```csharp
public class WhileExpression : IExpression {
    public ICondition Condition { get; set; }
    public List<IExpression> Body { get; set; }

    public void Execute(GameContext ctx) {
        while (Condition.Evaluate(ctx)) {
            foreach (var stmt in Body) {
                stmt.Execute(ctx);
            }
            // Wichtig: Endlosschleifen-Schutz!
            if (ctx.StepCount++ > 1000) break;
        }
    }
}
```

---

### 2.4 Stack-Maschine

**ABNF:**
```abnf
program ::= { statement }
statement ::= "PUSH" number
            | "POP"
            | "ADD"
            | "SUB"
            | "MUL"
            | "DIV"
            | "PRINT"
            | "DUP"
            | "SWAP"
number ::= ["-"] digit { digit }
```

**Beispiele:**
```
PUSH 5
PUSH 3
ADD
PRINT       # Ausgabe: 8

PUSH 10
DUP
MUL         # 10 * 10
PRINT       # Ausgabe: 100
```

**Konzepte:**
- Stack-Operationen
- Keine verschachtelten Strukturen
- Sehr einfach zu parsen (nur Tokens)

**Execute-Logik:**
```csharp
public class AddExpression : IExpression {
    public void Execute(StackContext ctx) {
        int b = ctx.Stack.Pop();
        int a = ctx.Stack.Pop();
        ctx.Stack.Push(a + b);
    }
}
```

---

## Konzept-Übersicht

| Konzept | Taschenrechner-Typ | RobotProgramming-Typ |
|---------|-------------------|---------------------|
| **Operator-Präzedenz** | ✅ Mehrere Ebenen | ❌ |
| **Assoziativität** | ✅ Links/Rechts | ❌ |
| **Verschachtelte Blöcke** | ❌ (nur Klammern) | ✅ REPEAT, IF, WHILE |
| **Rekursion** | ✅ Ausdrücke in Klammern | ✅ Blöcke in Blöcken |
| **Parameter** | ✅ Zahlen, Variablen | ✅ Zahlen, Strings, Enums |
| **Bedingungen** | ❌ | ✅ IF, WHILE |
| **Postfix-Operatoren** | ✅ Fakultät | ❌ |
| **Enumerations** | ❌ | ✅ Direction, Item |

---

## Wahrscheinlichkeit im Test

### Sehr wahrscheinlich (einfach)
- **Taschenrechner mit Modulo**: `+`, `-`, `*`, `/`, `%`
- **RobotProgramming mit IF**: Wie das Original + IF-Statement
- **Turtle Graphics**: FORWARD, BACK, LEFT, RIGHT + REPEAT

### Wahrscheinlich (mittel)
- **Logische Ausdrücke**: AND, OR, NOT
- **Taschenrechner mit Fakultät**: Postfix-Operator `!`
- **Textverarbeitung**: String-Parameter

### Weniger wahrscheinlich (schwer)
- **Vergleiche + Logik kombiniert**: Mehrere Präzedenz-Ebenen
- **WHILE-Schleifen**: Komplexere Kontrollstrukturen
- **Stack-Maschine**: Ganz anderes Konzept

---

## Tipps zur Vorbereitung

### 1. Beide Patterns beherrschen

**Client Parsing:**
```csharp
interface IExpression {
    void Parse(ref List<Token> tokens);
    void Execute(Context ctx);
}
```

**No Client Parsing:**
```csharp
interface IExpression {
    void Execute(Context ctx);
}
// Separater Parser baut den Baum
```

### 2. Wichtige Patterns

**Links-assoziativ (`while`):**
```csharp
IExpression left = ParseNext();
while (Match(Operator)) {
    Consume();
    IExpression right = ParseNext();
    left = new OpNode(left, right);  // left neu zuweisen!
}
return left;
```

**Rechts-assoziativ (`if` + Rekursion):**
```csharp
IExpression left = ParseNext();
if (Match(Operator)) {
    Consume();
    IExpression right = ParseSameLevel();  // Rekursion!
    return new OpNode(left, right);
}
return left;
```

**Block parsen:**
```csharp
List<IExpression> statements = new List<IExpression>();
Consume();  // {
while (!Match(CloseBrace)) {
    statements.Add(ParseStatement());
}
Consume();  // }
```

### 3. ABNF-Notation üben

```abnf
{ }   → 0 oder mehr Wiederholungen (while im Code)
[ ]   → 0 oder 1 mal (if im Code)
|     → Alternative (oder)
::=   → Definiert Regel
```

### 4. Typische Fehler vermeiden

❌ **Falsch:** Operatoren auf falscher Ebene
```csharp
// FALSCH: * und + auf gleicher Ebene
while (Match(Plus) || Match(Multiply)) { ... }
```

✅ **Richtig:** Separate Ebenen
```csharp
// ParseExpression: +, -
// ParseTerm: *, /
```

❌ **Falsch:** `^` links-assoziativ
```csharp
while (Match(Power)) { ... }  // → (2^3)^2 = 64
```

✅ **Richtig:** `^` rechts-assoziativ
```csharp
if (Match(Power)) {
    right = ParsePower();  // Rekursion → 2^(3^2) = 512
}
```

### 5. Testing-Strategie

1. **Einfache Fälle zuerst:**
   - `3 + 5`
   - `10 * 2`

2. **Assoziativität testen:**
   - `3 - 2 - 1` → `(3-2)-1` = 0
   - `2 ^ 3 ^ 2` → `2^(3^2)` = 512

3. **Verschachtelung testen:**
   - `(3 + 5) * 2`
   - `REPEAT 2 { REPEAT 3 { ... } }`

4. **Edge Cases:**
   - Leerer Block: `REPEAT 5 { }`
   - Nur ein Element: `42`
   - Viele Operatoren: `1+2+3+4+5`

---

## Cheat Sheet: Präzedenz-Tabellen

### Taschenrechner (klassisch)
| Priorität | Operatoren | Methode |
|-----------|-----------|---------|
| 1 (niedrig) | `+`, `-` | ParseExpression |
| 2 | `*`, `/`, `%` | ParseTerm |
| 3 | `^` | ParsePower |
| 4 (hoch) | `!`, Zahlen, Variablen, `()` | ParseFactor |

### Logik + Vergleich
| Priorität | Operatoren | Methode |
|-----------|-----------|---------|
| 1 (niedrig) | `OR` | ParseExpression |
| 2 | `AND` | ParseOrTerm |
| 3 | `<`, `>`, `==`, `!=` | ParseComparison |
| 4 | `+`, `-` | ParseTerm |
| 5 (hoch) | Zahlen, Variablen, `()` | ParseFactor |

---

Viel Erfolg! 🍀
