// Úloha: Posune binární číslo doleva o jeden bit.
// V binární interpretaci jde o násobení dvěma.
// Například 1011 -> 10110.

alphabet = {0,1}
tapeAlphabet = {0,1,#}
blank = #

(q0, 0) = (q0, 0, R)
(q0, 1) = (q0, 1, R)
(q0, #) = (qF, 0, S)

w = 1011
