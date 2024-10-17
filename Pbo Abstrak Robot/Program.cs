using System;
using System.Collections.Generic;

// Interface untuk Kemampuan
interface IKemampuan
{
    void Gunakan(); // Metode untuk menggunakan kemampuan
    bool BisaDigunakan(); // Mengecek apakah cooldown selesai
}

// Abstract Class Robot
abstract class Robot
{
    public string nama;
    public int energi;
    public int armor;
    public int serangan;

    public Robot(string nama, int energi, int armor, int serangan)
    {
        this.nama = nama;
        this.energi = energi;
        this.armor = armor;
        this.serangan = serangan;
    }

    public abstract void Serang(Robot target);
    public abstract void GunakanKemampuan(IKemampuan kemampuan);

    public void CetakInformasi()
    {
        Console.WriteLine($"Nama: {nama}");
        Console.WriteLine($"Energi: {energi}");
        Console.WriteLine($"Armor: {armor}");
        Console.WriteLine($"Serangan: {serangan}");
    }

    public void KurangiEnergi(int jumlah)
    {
        energi -= jumlah;
        if (energi < 0) energi = 0;
    }

    public bool MasihHidup()
    {
        return energi > 0;
    }

    public void PulihkanEnergi()
    {
        energi += 10; // Memulihkan 10 energi setiap akhir giliran
        if (energi > 100) energi = 100; // Batas maksimum energi
        Console.WriteLine($"{nama} memulihkan 10 energi. Energi sekarang: {energi}");
    }
}

// Implementasi Robot Biasa
class RobotBiasa : Robot
{
    public RobotBiasa(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan) { }

    public override void Serang(Robot target)
    {
        if (!MasihHidup())
        {
            Console.WriteLine($"{nama} tidak bisa menyerang, sudah kehabisan energi.");
            return;
        }

        int damage = serangan - target.armor;
        if (damage < 0) damage = 0;
        target.KurangiEnergi(damage);

        Console.WriteLine($"{nama} menyerang {target.nama} dengan damage {damage}");
    }

    public override void GunakanKemampuan(IKemampuan kemampuan)
    {
        if (kemampuan.BisaDigunakan())
        {
            kemampuan.Gunakan();
        }
        else
        {
            Console.WriteLine($"{nama} belum bisa menggunakan kemampuan ini.");
        }
    }
}

// Implementasi Bos Robot
class BosRobot : Robot
{
    private int pertahanan;

    public BosRobot(string nama, int energi, int armor, int serangan, int pertahanan) : base(nama, energi, armor, serangan)
    {
        this.pertahanan = pertahanan;
    }

    public override void Serang(Robot target)
    {
        if (!MasihHidup())
        {
            Console.WriteLine($"{nama} tidak bisa menyerang, sudah kehabisan energi.");
            return;
        }
        int damage = serangan - (target.armor / 2); // Bos menyerang lebih kuat
        if (damage < 0) damage = 0;
        target.KurangiEnergi(damage);
        Console.WriteLine($"{nama} menyerang {target.nama} dengan damage {damage}");
    }

    public override void GunakanKemampuan(IKemampuan kemampuan)
    {
        if (kemampuan.BisaDigunakan())
        {
            kemampuan.Gunakan();
        }
        else
        {
            Console.WriteLine($"{nama} belum bisa menggunakan kemampuan ini.");
        }
    }

    public void Diserang(Robot penyerang)
    {
        int damage = penyerang.serangan - pertahanan;
        if (damage < 0) damage = 0;
        KurangiEnergi(damage);
        Console.WriteLine($"{nama} diserang oleh {penyerang.nama} dengan damage {damage}");
        if (energi <= 0) Mati();
    }

    public void Mati()
    {
        Console.WriteLine($"{nama} telah dikalahkan!");
    }
}

// Implementasi Kemampuan Perbaikan
class Perbaikan : IKemampuan
{
    private Robot robot;
    private int pemulihan;
    private int cooldown;
    private int cooldownCounter;

    public Perbaikan(Robot robot, int pemulihan)
    {
        this.robot = robot ?? throw new ArgumentNullException(nameof(robot));
        this.pemulihan = pemulihan;
        this.cooldown = 3;
        this.cooldownCounter = 0;
    }

    public void Gunakan()
    {
        robot.energi += pemulihan;
        Console.WriteLine($"{robot.nama} memulihkan {pemulihan} energi.");
        cooldownCounter = cooldown;
    }

    public bool BisaDigunakan()
    {
        return cooldownCounter <= 0;
    }

    public void KurangiCooldown()
    {
        if (cooldownCounter > 0)
        {
            cooldownCounter--;
        }
    }
}

// Implementasi Kemampuan Serangan Listrik
class SeranganListrik : IKemampuan
{
    private Robot target;
    private int damage;
    private int cooldown;
    private int cooldownCounter;

    public SeranganListrik(Robot target, int damage)
    {
        this.target = target;
        this.damage = damage;
        this.cooldown = 2;
        this.cooldownCounter = 0;
    }

    public void Gunakan()
    {
        target.KurangiEnergi(damage);
        Console.WriteLine($"Serangan listrik menyerang {target.nama} dengan damage {damage}");
        cooldownCounter = cooldown;
    }

    public bool BisaDigunakan()
    {
        return cooldownCounter <= 0;
    }

    public void KurangiCooldown()
    {
        if (cooldownCounter > 0)
        {
            cooldownCounter--;
        }
    }
}

// Kelas Simulator Pertarungan dengan CRUD
class SimulatorPertarungan
{
    private List<Robot> daftarRobot = new List<Robot>();

    public void TambahRobot(Robot robot)
    {
        daftarRobot.Add(robot);
        Console.WriteLine($"{robot.nama} telah ditambahkan ke daftar robot.");
    }

    public void TampilkanDaftarRobot()
    {
        Console.WriteLine("Daftar Robot:");
        for (int i = 0; i < daftarRobot.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {daftarRobot[i].nama} (Energi: {daftarRobot[i].energi}, Armor: {daftarRobot[i].armor}, Serangan: {daftarRobot[i].serangan})");
        }
    }

    public void HapusRobot(int indeks)
    {
        if (indeks >= 0 && indeks < daftarRobot.Count)
        {
            Console.WriteLine($"{daftarRobot[indeks].nama} telah dihapus.");
            daftarRobot.RemoveAt(indeks);
        }
        else
        {
            Console.WriteLine("Indeks tidak valid.");
        }
    }

    public void Pertarungan(int indeksA, int indeksB)
    {
        if (indeksA >= 0 && indeksA < daftarRobot.Count && indeksB >= 0 && indeksB < daftarRobot.Count)
        {
            Robot robotA = daftarRobot[indeksA];
            Robot robotB = daftarRobot[indeksB];

            Console.WriteLine($"{robotA.nama} melawan {robotB.nama}");
            robotA.Serang(robotB);
            if (robotB.MasihHidup()) robotB.Serang(robotA);

            robotA.CetakInformasi();
            robotB.CetakInformasi();
        }
        else
        {
            Console.WriteLine("Indeks robot tidak valid.");
        }
    }

    public static void Main(string[] args)
    {
        SimulatorPertarungan simulator = new SimulatorPertarungan();

        while (true)
        {
            Console.WriteLine("\n--- Menu Utama ---");
            Console.WriteLine("1. Tambah Robot");
            Console.WriteLine("2. Lihat Daftar Robot");
            Console.WriteLine("3. Hapus Robot");
            Console.WriteLine("4. Pertarungan");
            Console.WriteLine("5. Keluar");
            Console.Write("Pilih opsi: ");
            int pilihan = int.Parse(Console.ReadLine());

            switch (pilihan)
            {
                case 1:
                    Console.Write("Masukkan nama robot: ");
                    string nama = Console.ReadLine();
                    Console.Write("Masukkan energi: ");
                    int energi = int.Parse(Console.ReadLine());
                    Console.Write("Masukkan armor: ");
                    int armor = int.Parse(Console.ReadLine());
                    Console.Write("Masukkan serangan: ");
                    int serangan = int.Parse(Console.ReadLine());

                    Robot robotBaru = new RobotBiasa(nama, energi, armor, serangan);
                    simulator.TambahRobot(robotBaru);
                    break;

                case 2:
                    simulator.TampilkanDaftarRobot();
                    break;

                case 3:
                    simulator.TampilkanDaftarRobot();
                    Console.Write("Masukkan indeks robot yang ingin dihapus: ");
                    int indeksHapus = int.Parse(Console.ReadLine()) - 1;
                    simulator.HapusRobot(indeksHapus);
                    break;

                case 4:
                    simulator.TampilkanDaftarRobot();
                    Console.Write("Pilih robot pertama (indeks): ");
                    int robotA = int.Parse(Console.ReadLine()) - 1;
                    Console.Write("Pilih robot kedua (indeks): ");
                    int robotB = int.Parse(Console.ReadLine()) - 1;
                    simulator.Pertarungan(robotA, robotB);
                    break;

                case 5:
                    return;

                default:
                    Console.WriteLine("Pilihan tidak valid.");
                    break;
            }
        }
    }
}
