import 'package:flutter/material.dart';

import '../../core/app_state.dart';
import '../../core/theme/colors.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;
    final rol = appState.yoneticiMi ? 'Yönetici' : 'Kullanıcı';

    return Scaffold(
      appBar: AppBar(title: const Text('Profil')),
      body: ListView(
        padding: const EdgeInsets.all(20),
        children: [
          const CircleAvatar(
            radius: 44,
            backgroundColor: AppColors.primary,
            child: Icon(Icons.person, color: Colors.white, size: 44),
          ),
          const SizedBox(height: 12),
          Text(
            appState.email,
            textAlign: TextAlign.center,
            style: textTheme.titleMedium,
          ),
          const SizedBox(height: 8),
          Center(child: Chip(label: Text(rol))),
          const SizedBox(height: 24),
          const Card(
            child: Column(
              children: [
                ListTile(
                  leading: Icon(Icons.school_outlined),
                  title: Text('Bölüm'),
                  subtitle: Text('Hafta 4’te eklenecek'),
                ),
                Divider(height: 1),
                ListTile(
                  leading: Icon(Icons.layers_outlined),
                  title: Text('Sınıf'),
                  subtitle: Text('Hafta 4’te eklenecek'),
                ),
                Divider(height: 1),
                ListTile(
                  leading: Icon(Icons.percent_outlined),
                  title: Text('Not ortalaması'),
                  subtitle: Text('Hafta 4’te eklenecek'),
                ),
              ],
            ),
          ),
          const SizedBox(height: 24),
          OutlinedButton.icon(
            onPressed: () async {
              await appState.cikisYap();
            },
            icon: const Icon(Icons.logout),
            label: const Text('Çıkış yap'),
            style: OutlinedButton.styleFrom(
              minimumSize: const Size.fromHeight(52),
              foregroundColor: AppColors.error,
              side: const BorderSide(color: AppColors.error),
            ),
          ),
        ],
      ),
    );
  }
}
