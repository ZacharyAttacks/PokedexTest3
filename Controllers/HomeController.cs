using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using LegendaryPokedex.Models;
using Pokedex.Data;

namespace LegendaryPokedex.Controllers
{
    public class HomeController : Controller
    {
        private readonly PokedexContext _context;
        private readonly IWebHostEnvironment _env;

        public HomeController(PokedexContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ Load Pokémon data from JSON file
        private Dictionary<string, Pokemon> LoadPokemonData()
        {
            var filePath = Path.Combine(_env.WebRootPath, "data/pokemon_data.json");

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("The pokemon_data.json file is missing. Please ensure it exists in /wwwroot/data/");
            }

            var jsonData = System.IO.File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, Pokemon>>(jsonData);
        }

        // ✅ Display main Pokédex page (All Pokémon in Database)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pokemon.ToListAsync());
        }

        // ✅ Display the Add Pokémon page (List of Legendary Pokémon)
        public IActionResult AddPokemon()
        {
            var pokemonList = LoadPokemonData().Keys.ToList();
            return View(pokemonList);
        }

        // ✅ Add Pokémon (Automatically fills in stats from JSON file)
        [HttpPost]
        public async Task<IActionResult> AddPokemon(string selectedPokemon)
        {
            var allPokemonData = LoadPokemonData();

            if (allPokemonData.ContainsKey(selectedPokemon))
            {
                var pokemonData = allPokemonData[selectedPokemon];

                var pokemon = new Pokemon
                {
                    Name = selectedPokemon,
                    Typing = pokemonData.Typing,
                    Description = pokemonData.Description,
                    HP = pokemonData.HP,
                    Attack = pokemonData.Attack,
                    Defense = pokemonData.Defense,
                    SpecialAttack = pokemonData.SpecialAttack,
                    SpecialDefense = pokemonData.SpecialDefense,
                    Speed = pokemonData.Speed,
                    Move1 = "",
                    Move2 = "",
                    Move3 = "",
                    Move4 = ""
                };

                _context.Pokemon.Add(pokemon);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }


        // ✅ View Pokémon Details
        public IActionResult ViewPokemon(int id)
        {
            var pokemon = _context.Pokemon.FirstOrDefault(p => p.Id == id);

            if (pokemon == null)
            {
                return NotFound();
            }

            // ✅ Remove JSON dependency - Just set default "Normal" type for now
            pokemon.Move1Type = "normal";
            pokemon.Move2Type = "normal";
            pokemon.Move3Type = "normal";
            pokemon.Move4Type = "normal";

            return View(pokemon);
        }


        // ✅ Fetches the correct move type from the database


        // ✅ Edit Pokémon (ONLY Moveset & Form)
        public async Task<IActionResult> EditPokemon(int id)
        {
            var pokemon = await _context.Pokemon.FindAsync(id);
            return View(pokemon);
        }

        [HttpPost]


        [HttpPost]
        [HttpPost]
        public IActionResult ToggleFavorite(int id)
        {
            var pokemon = _context.Pokemon.Find(id);
            if (pokemon == null)
            {
                return NotFound();
            }

            // Toggle the favorite status
            pokemon.IsFavorite = !pokemon.IsFavorite;
            _context.SaveChanges();

            return RedirectToAction("Index"); // ✅ Redirects back to Index instead of ViewPokemon
        }



        // ✅ Delete Pokémon
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpDelete] // This allows DELETE requests
        public IActionResult DeletePokemon(int id)
        {
            var pokemon = _context.Pokemon.Find(id);
            if (pokemon == null)
            {
                return NotFound();
            }

            _context.Pokemon.Remove(pokemon);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // ✅ Toggle Favorite Pokémon
        public async Task<IActionResult> FavoritePokemon(int id)
        {
            var pokemon = await _context.Pokemon.FindAsync(id);
            if (pokemon != null)
            {
                pokemon.IsFavorite = !pokemon.IsFavorite;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ✅ Select a Move for Pokémon (Only Shows Pokémon's Learnable Moves)
        public IActionResult SelectMove(int pokemonId, int slot)
        {
            var filePath = Path.Combine(_env.WebRootPath, "data/moves.json");

            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("The moves.json file is missing. Please make sure it exists in /wwwroot/data/");
            }

            var jsonData = System.IO.File.ReadAllText(filePath);
            var allMoves = JsonConvert.DeserializeObject<Dictionary<string, List<Move>>>(jsonData) ?? new Dictionary<string, List<Move>>();

            var pokemon = _context.Pokemon.Find(pokemonId);
            if (pokemon == null)
            {
                return NotFound();
            }

            // 🔥 Get only the moves for the selected Pokémon
            var movesList = allMoves.ContainsKey(pokemon.Name) ? allMoves[pokemon.Name] : new List<Move>();

            ViewBag.PokemonId = pokemonId;
            ViewBag.Slot = slot;

            return View(movesList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignMove(int pokemonId, int slot, string moveName)
        {
            var pokemon = await _context.Pokemon.FindAsync(pokemonId);
            if (pokemon != null)
            {
                // 🔥 Prevent Duplicate Moves
                var currentMoves = new List<string> { pokemon.Move1, pokemon.Move2, pokemon.Move3, pokemon.Move4 };

                if (currentMoves.Contains(moveName))
                {
                    TempData["Error"] = "This Pokémon already knows this move!";
                    return RedirectToAction("SelectMove", new { pokemonId, slot });
                }

                // ✅ Assign the move to the correct slot
                switch (slot)
                {
                    case 1: pokemon.Move1 = moveName; break;
                    case 2: pokemon.Move2 = moveName; break;
                    case 3: pokemon.Move3 = moveName; break;
                    case 4: pokemon.Move4 = moveName; break;
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("EditPokemon", new { id = pokemonId });
        }
    }
}