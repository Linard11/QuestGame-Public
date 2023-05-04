=== donaldduck ===
{eventDone: -> already_talkedTo}
-> doorQuest

= doorQuest
Donald Duck: Möchtest du die Türen verschwinden lassen?
* [Sure, ill do it.] -> eventDone
* I rather not.
Doandl Duck: Dann bleibt der Raum geschlossen.
-> END

= eventDone
Donald Duck: Verschwindibuss...
~ Event("door_hide")
-> END

= already_talkedTo
Donald Duck: ....
-> END