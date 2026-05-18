// Úloha: Provede bitovou negaci binárního vstupu.

alphabet = {0,1}
tapeAlphabet = {0,1,#}
blank = #

(q0, 0) = (q0, 1, R)
(q0, 1) = (q0, 0, R)
(q0, #) = (qF, #, S)

w = 101100
