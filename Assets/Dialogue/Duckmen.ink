=== duckmen ===
{island_accept: -> island_follow_up}
{island.decline: -> island_retry}
-> island


= island
Duckmen: Hey there fellow humanoid! What brings you to these lands?
Robot: Just passing through, looking at the scenery.
Duckmen: Then you might want to check out a pretty cool island over there. It has a lovely small forest on it.
* [Sure! Sounds good.] -> island_accept
* (decline) [I rather not.]
    Duckmen: That's a bummer.
    Well if you reconsider, just say the word.
-> END

= island_retry
Robot: About island...
Duckmen: Did you reconsider? Just say the word, and I'll tell you how to get over there.
* [Yes] -> island_accept
+ [I still don't want to.]
    Duckmen: To each their own...
-> END

= island_accept
Duckmen: Great. Just head overe to the bridge.
There should be a button near it you can press to extend the bridge.
It's a bit stuck, so you really have to jam it in to activate it.
Robot: Thanks for the info!
~ Event("unlock_bridge")
-> END

= island_follow_up
Duckmen: Did you already check out the island?
Just head over to the bridge and press the button.
-> END