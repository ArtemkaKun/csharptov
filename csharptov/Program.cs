namespace csharptov;

public abstract class Entity
{
	public readonly string name;

	protected Entity(string name)
	{
		this.name = name;
	}
}

public class Enemy : Entity
{
	public readonly uint defence;

	public Enemy(string name, uint defence) : base(name)
	{
		this.defence = defence;
	}
}

public abstract class Human : Entity
{
	protected uint xp;

	protected readonly uint weaponQuality;

	protected Human(string name, uint xp, uint weaponQuality) : base(name)
	{
		this.xp = xp;
		this.weaponQuality = weaponQuality;
	}
}

public class Player : Human
{
	private readonly float power;

	public Player(string name, uint xp, uint weaponQuality, float power) : base(name, xp, weaponQuality)
	{
		this.power = power;
	}

	public float GetAttackPower()
	{
		return power * xp * weaponQuality;
	}

	public void IncrementXP()
	{
		xp += 1;
	}
}

public interface IEnemy
{
	public uint GetDefence();
	public string GetName();
}

public class Wolf : Enemy, IEnemy
{
	private readonly uint agile;

	public Wolf(string name, uint defence, uint agile) : base(name, defence)
	{
		this.agile = agile;
	}

	public uint GetDefence()
	{
		return defence * agile;
	}

	public string GetName()
	{
		return name;
	}
}

public class Guard : Human, IEnemy
{
	private readonly Enemy enemyStats;

	public Guard(string name, uint xp, uint weaponQuality, uint defence) : base(name, xp, weaponQuality)
	{
		enemyStats = new Enemy(name, defence);
	}

	public uint GetDefence()
	{
		return enemyStats.defence * weaponQuality * xp;
	}

	public string GetName()
	{
		return name;
	}
}

public static class Program
{
	private const string HIT_COMMAND = "hit";
	private const string EXIT_COMMAND = "exit";
	private const string DEFAULT_PLAYER_NAME = "Player";
	private const uint START_XP = 1;
	private const uint MIN_START_WEAPON_QUALITY = 1;
	private const uint MAX_START_WEAPON_QUALITY = 5;
	private const uint MAX_ENEMY_DEFENCE = 5;
	private const uint MAX_AGILE = 10;
	private const uint MAX_ENEMY_WEAPON_QUALITY = 10;
	private const uint MAX_ENEMY_XP = 10;

	private static Player playerInstance = null!;

	public static void Main()
	{
		PrintGameStartMessages();
		CreatePlayerCharacter();
		StartMainGameLoop();
	}

	private static void PrintGameStartMessages()
	{
		Console.WriteLine("Tiny RPG");
		Console.WriteLine($"Print \"{HIT_COMMAND}\" to attack an enemy");
		Console.WriteLine($"Print \"{EXIT_COMMAND}\" to finish the game");
	}

	private static void CreatePlayerCharacter()
	{
		Console.WriteLine("Name of your character:");
		playerInstance = new Player(GetPlayerName(), START_XP, GetPlayerStartWeaponQuality(), GetStartPower());
		Console.WriteLine($"Your character - {playerInstance.name}, attack power - {playerInstance.GetAttackPower()}");
	}

	private static string GetPlayerName()
	{
		return Console.ReadLine() ?? DEFAULT_PLAYER_NAME;
	}

	private static uint GetPlayerStartWeaponQuality()
	{
		return (uint) Random.Shared.Next((int) MIN_START_WEAPON_QUALITY, (int) MAX_START_WEAPON_QUALITY);
	}

	private static float GetStartPower()
	{
		return Random.Shared.NextSingle() + 1.0f;
	}

	private static void StartMainGameLoop()
	{
		while (true)
		{
			IEnemy currentEnemy = CreateNewEnemy();
			Console.WriteLine($"Your next enemy - {currentEnemy.GetName()}, defence - {currentEnemy.GetDefence()}");
			Console.WriteLine("What will you do?");
			string? userInput = Console.ReadLine();

			if (userInput != null)
			{
				if (userInput == EXIT_COMMAND)
				{
					break;
				}

				if (userInput == HIT_COMMAND)
				{
					TryKillEnemy(currentEnemy);
				}
			}
			else
			{
				break;
			}
		}
	}

	private static IEnemy CreateNewEnemy()
	{
		int randomNumber = Random.Shared.Next(2);
		return randomNumber == 0 ? CreateNewWolf() : CreateNewGuard();
	}

	private static Wolf CreateNewWolf()
	{
		return new Wolf("Wolf", GetRandomDefence(), GetRandomAgile());
	}

	private static uint GetRandomDefence()
	{
		return (uint) Random.Shared.Next((int) MAX_ENEMY_DEFENCE);
	}

	private static uint GetRandomAgile()
	{
		return (uint) Random.Shared.Next((int) MAX_AGILE);
	}

	private static Guard CreateNewGuard()
	{
		return new Guard("Guard", GetRandomXP(), GetRandomEnemyWeaponQuality(), GetRandomDefence());
	}

	private static uint GetRandomEnemyWeaponQuality()
	{
		return (uint) Random.Shared.Next((int) MAX_ENEMY_WEAPON_QUALITY);
	}

	private static uint GetRandomXP()
	{
		return (uint) Random.Shared.Next((int) MAX_ENEMY_XP);
	}
	
	private static void TryKillEnemy(IEnemy currentEnemy)
	{
		bool canKillEnemy = currentEnemy.GetDefence() <= playerInstance.GetAttackPower();

		if (canKillEnemy == true)
		{
			Console.WriteLine("Enemy was killed!");
			playerInstance.IncrementXP();
		}
		else
		{
			Console.WriteLine($"Your attack is too low ({playerInstance.GetAttackPower()})");
		}
	}
}