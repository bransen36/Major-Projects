#!/usr/bin/python
# hangmanGame.py

import random

WORD_LISTS = [
    # List of animals
    '''cow sheep pig horse dog elephant giraffe pony frog turtle monkey tiger lion platypus toucan snake whale rhinoceros bear cougar wildcat chicken zebra goat deer llama emu ram ostrich boar elk moose goose duck penguin flamingo donkey cat cheetah leopard wolf fox kangaroo jaguar bison otter beaver porcupine alligator crocodile hawk eagle parrot dolphin walrus octopus scorpion meerkat hyena koala bat'''.lower().split(),

    # List of cars
    '''toyota honda ford chevrolet nissan bmw mercedes audi volkswagen hyundai kia subaru mazda dodge jeep chrysler ram buick cadillac acura lexus infiniti mitsubishi tesla volvo porsche land rover jaguar mini fiat alfa romeo maserati ferrari lamborghini bentley bugatti mclaren genesis peugeot citroën renault opel seat skoda suzuki isuzu mahindra tata rivian lucid polestar'''.lower().split(),

    # List of sports
    '''soccer basketball baseball american football tennis golf volleyball cricket rugby hockey ice hockey track and field swimming boxing wrestling mma gymnastics cycling rowing badminton table tennis softball handball lacrosse water polo surfing skateboarding snowboarding skiing bowling fishing archery fencing weightlifting powerlifting rock climbing equestrian auto racing motocross triathlon decathlon ultimate dodgeball kickball bocce chess esports poker darts shuffleboard'''.lower().split(),

    # List of foods
    '''apples bananas carrots potatoes tomatoes spinach onions broccoli lettuce oranges rice pasta bread oatmeal corn chicken beef eggs milk cheese lentils chickpeas black beans peanuts almonds butter yogurt sugar chocolate pizza fish shrimp turkey pork cabbage cucumbers peppers strawberries blueberries grapes avocado mushrooms garlic ginger honey peanut butter mayonnaise mustard ketchup'''.lower().split()
]


def display_word_progress(word, guessed_letters):
    """Displays the word with guessed letters revealed and underscores for unknown letters."""
    return ' '.join(char if char in guessed_letters else '_' for char in word)


def get_valid_guess(guessed_letters):
    """Prompts the user for a valid single-letter guess."""
    while True:
        guess = input("Enter a letter: ").lower()
        if not guess.isalpha():
            print("❌ Enter only a letter!")
        elif len(guess) > 1:
            print("❌ Enter only a single letter!")
        elif guess in guessed_letters:
            print("❌ You've already guessed that letter!")
        else:
            return guess


def play_game():
    """Main function to play Hangman."""
    CHOSEN_LIST = random.choice(WORD_LISTS)
    word = random.choice(CHOSEN_LIST)

    # Ensure the word is a single word (no two-word phrases)
    while len(word.split()) > 1:
        word = random.choice(CHOSEN_LIST)

    guessed_letters = set()
    remaining_attempts = len(word) + 2

    print("\n🎮 Welcome to Hangman!")
    if CHOSEN_LIST == WORD_LISTS[0]:
        print(f"Hint: The word is an animal.")
    elif CHOSEN_LIST == WORD_LISTS[1]:
        print(f"Hint: The word is a car.")
    elif CHOSEN_LIST == WORD_LISTS[2]:
        print(f"Hint: The word is a sport.")
    else:
        print(f"Hint: The word is a food.")
    print(display_word_progress(word, guessed_letters))

    while remaining_attempts > 0:
        print(f"\nAttempts left: {remaining_attempts}")
        guess = get_valid_guess(guessed_letters)
        guessed_letters.add(guess)

        if guess in word:
            print("✅ Correct!")
        else:
            print("❌ Incorrect!")
            remaining_attempts -= 1

        # Display the updated word progress
        current_progress = display_word_progress(word, guessed_letters)
        print(current_progress)

        # Check if the player has guessed the entire word
        if "_" not in current_progress:
            print(f"\n🎉 Congratulations! You guessed the word: {word}")
            break
    else:
        print(f"\n❌ You lost! The word was: {word}")

    # Play again prompt
    while True:
        choice = input("Play again? (y/n): ").lower()
        if choice == "y":
            play_game()
            break
        elif choice == "n":
            print("Thanks for playing! 👋")
            break
        else:
            print("❌ Invalid choice! Enter 'y' or 'n'.")


if __name__ == "__main__":
    try:
        play_game()
    except KeyboardInterrupt:
        print("\n🚪 Exiting game. Goodbye!")
