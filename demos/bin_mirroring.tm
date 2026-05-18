// Úloha: Připojí za binární vstup jeho zrcadlově otočenou kopii.
// Například 1100101 -> 11001011010011.

alphabet = {0,1}
tapeAlphabet = {0,1,#,o,i}
blank = #

(q0,0)= (q0,0,R)
(q0,1) = (q0,1,R)
(q0,o) = (q0,0,R)
(q0,i) = (q0,1,R)
(q0, #)=(q1, #,L)

// q1 prochází zkopírovanou část zprava doleva.
// První nezpracovanou číslici označí pomocným symbolem o/i.

(q1,0) = (q2,o, R)
(q1,1) = (q3,i, R)
(q1,o) = (q1,o,L)
(q1,i) = (q1,i,L)

// q2 a q3 doběhnou na pravý konec a připíšou kopii označené číslice.

(q2, #) = (q1,o,L)
(q3, #) = (q1,i,L)

// Neprázdná políčka se při běhu doprava pouze přeskakují.

(q2,0) = (q2,0, R)
(q2,1) = (q2,1, R)
(q2,o) = (q2,o, R)
(q2,i) = (q2,i, R)
(q3,0) = (q3,0, R)
(q3,1) = (q3,1, R)
(q3,o) = (q3,o, R)
(q3,i) = (q3,i, R)

// Po zpracování celého vstupu q4 odstraní pomocné značky.

(q1, #) = (q4, #, R)
(q4,o) = (q4,0, R)
(q4,i) = (q4,1, R)
(q4,0) = (q4,0,L)
(q4,1) = (q4,1,L)
(q4, #) = (qF, #, R)

w = 1100101
