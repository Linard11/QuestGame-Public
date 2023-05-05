EXTERNAL Event(eventName)
EXTERNAL Get_State(id)
EXTERNAL Add_State(id, amount)

//Invokes a generic event with a string identifier
=== function Event(eventName)
// Fallback in case actual function is not available
[Event: {eventName}]

=== function Get_State(id)
[ GET STATE: {id} ]
~ return 0

=== function Add_State(id, amount)
[ SET STATE: {id} - VALUE: {amount} ]0