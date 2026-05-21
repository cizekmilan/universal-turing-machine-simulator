// Úloha: Ověří, zda je binární vstup palindrom.
// Výsledkem je 1 pro palindrom a 0 pro nepalindrom.
// Například 10101 -> 1.

alphabet = {0,1}
tapeAlphabet = {0,1,#,x,y}
blank = #

// q0 hledá zleva první dosud nezpracovaný symbol.
// Nulu označí symbolem x, jedničku symbolem y.

(q0, x) = (q0, x, R)
(q0, y) = (q0, y, R)
(q0, 0) = (q1, x, R)
(q0, 1) = (q2, y, R)
(q0, #) = (q6, #, L)

// q1 a q2 doběhnou na pravý konec slova podle toho,
// zda se má porovnávat nula nebo jednička.

(q1, 0) = (q1, 0, R)
(q1, 1) = (q1, 1, R)
(q1, x) = (q1, x, R)
(q1, y) = (q1, y, R)
(q1, #) = (q3, #, L)

(q2, 0) = (q2, 0, R)
(q2, 1) = (q2, 1, R)
(q2, x) = (q2, x, R)
(q2, y) = (q2, y, R)
(q2, #) = (q4, #, L)

// q3 hledá zprava odpovídající nulu.
// q4 hledá zprava odpovídající jedničku.

(q3, x) = (q3, x, L)
(q3, y) = (q3, y, L)
(q3, 0) = (q5, x, L)
(q3, 1) = (q9, 1, L)
(q3, #) = (q6, #, R)

(q4, x) = (q4, x, L)
(q4, y) = (q4, y, L)
(q4, 1) = (q5, y, L)
(q4, 0) = (q9, 0, L)
(q4, #) = (q6, #, R)

// q5 se vrací na levý okraj a pokračuje dalším porovnáním.

(q5, 0) = (q5, 0, L)
(q5, 1) = (q5, 1, L)
(q5, x) = (q5, x, L)
(q5, y) = (q5, y, L)
(q5, #) = (q0, #, R)

// q6, q7 a q8 čistí pásku a zapíší výsledek 1.
// q12 se vrátí zpět na zapsaný výsledek a tam stroj zastaví.

(q6, 0) = (q6, 0, L)
(q6, 1) = (q6, 1, L)
(q6, x) = (q6, x, L)
(q6, y) = (q6, y, L)
(q6, #) = (q7, #, R)

(q7, 0) = (q8, 1, R)
(q7, 1) = (q8, 1, R)
(q7, x) = (q8, 1, R)
(q7, y) = (q8, 1, R)
(q7, #) = (qF, 1, S)

(q8, 0) = (q8, #, R)
(q8, 1) = (q8, #, R)
(q8, x) = (q8, #, R)
(q8, y) = (q8, #, R)
(q8, #) = (q12, #, L)

(q12, #) = (q12, #, L)
(q12, 1) = (qF, 1, S)

// q9, q10 a q11 čistí pásku a zapíší výsledek 0.
// q13 se vrátí zpět na zapsaný výsledek a tam stroj zastaví.

(q9, 0) = (q9, 0, L)
(q9, 1) = (q9, 1, L)
(q9, x) = (q9, x, L)
(q9, y) = (q9, y, L)
(q9, #) = (q10, #, R)

(q10, 0) = (q11, 0, R)
(q10, 1) = (q11, 0, R)
(q10, x) = (q11, 0, R)
(q10, y) = (q11, 0, R)
(q10, #) = (qF, 0, S)

(q11, 0) = (q11, #, R)
(q11, 1) = (q11, #, R)
(q11, x) = (q11, #, R)
(q11, y) = (q11, #, R)
(q11, #) = (q13, #, L)

(q13, #) = (q13, #, L)
(q13, 0) = (qF, 0, S)

w = 10101
