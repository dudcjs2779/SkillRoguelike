INCLUDE globals.ink

{ pokemon_name == "": -> main | -> already_chose}

=== main ===
안녕 여기에 포켓몬이 3개가 있다.
Which pokemon do you choose?
    + [Charmander]
        -> chosen("Chamander")
    + [Bulbasaur]
        -> chosen("Bulbasaur")
    + [Squirtle]
        -> chosen("Squirtle")
        
=== chosen(pokemon) ===
~ pokemon_name = pokemon
You chose {pokemon}!
->END

=== already_chose ===
You already chose {pokemon_name}!
-> END