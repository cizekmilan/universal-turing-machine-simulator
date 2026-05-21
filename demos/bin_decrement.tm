// Úloha: Dekrementuje binární číslo o 1.
// Například 1000 -> 0111.

alphabet = {0,1}
tapeAlphabet = {0,1,#}
blank = #

(q0, 0) = (q0, 0, R)
(q0, 1) = (q0, 1, R)
(q0, #) = (q1, #, L)
(q1, 0) = (q1, 1, L)
(q1, 1) = (q2, 0, L)
(q1, #) = (qF, #, R)
(q2, 0) = (q2, 0, L)
(q2, 1) = (q2, 1, L)
(q2, #) = (qF, #, R)
w = 1000
