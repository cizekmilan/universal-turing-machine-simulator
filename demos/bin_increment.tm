// Úloha: Přičte 1 k binárnímu číslu.

alphabet = {0,1}
tapeAlphabet = {0,1,#}
blank = #

(q0, 1) = (q0, 1, R)
(q0, 0) = (q0, 0, R)
(q0, #) = (q1, #, L)
(q1, 0) = (q2, 1, L)
(q1, 1) = (q3, 0, L)
(q2, 1) = (q2, 1, L)
(q2, 0) = (q2, 0, L)
(q2, #) = (qF, #, R)
(q3, 1) = (q3, 0, L)
(q3, 0) = (q2, 1, L)
(q3, #) = (qF, 1, S)

w = 1011
