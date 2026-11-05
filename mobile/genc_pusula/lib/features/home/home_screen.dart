import 'package:flutter/material.dart';

import '../../core/app_state.dart';
import '../../core/theme/colors.dart';
import '../../core/widgets/empty_state.dart';
import '../../core/widgets/opportunity.dart';
import '../../shared/models/firsat_ozeti.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  late Future<List<FirsatOzeti>> _firsatlar = appState.firsatlariGetir();

  void _yenile() {
    setState(() {
      _firsatlar = appState.firsatlariGetir();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: FutureBuilder<List<FirsatOzeti>>(
          future: _firsatlar,
          builder: (context, snapshot) {
            if (snapshot.connectionState == ConnectionState.waiting) {
              return const Center(child: CircularProgressIndicator());
            }

            if (snapshot.hasError) {
              return EmptyState(
                icon: Icons.cloud_off_rounded,
                title: 'Fırsatlar yüklenemedi',
                message: snapshot.error.toString(),
                actionLabel: 'Tekrar dene',
                onAction: _yenile,
              );
            }

            final liste = snapshot.data ?? const <FirsatOzeti>[];
            if (liste.isEmpty) {
              return EmptyState(
                icon: Icons.inbox_outlined,
                title: 'Henüz fırsat yok',
                message: 'Yeni fırsatlar eklendiğinde burada görünecek.',
                actionLabel: 'Yenile',
                onAction: _yenile,
              );
            }

            return _liste(liste);
          },
        ),
      ),
    );
  }

  Widget _liste(List<FirsatOzeti> liste) {
    final textTheme = Theme.of(context).textTheme;
    final ad = appState.email.split('@').first;

    return ListView(
      padding: const EdgeInsets.fromLTRB(20, 16, 20, 24),
      children: [
        Row(
          children: [
            Expanded(
              child: Text(
                ad.isEmpty ? 'Merhaba 👋' : 'Merhaba, $ad 👋',
                style: textTheme.headlineSmall,
              ),
            ),
            IconButton(
              onPressed: _yenile,
              tooltip: 'Yenile',
              icon: const Icon(Icons.refresh),
            ),
            CircleAvatar(
              radius: 20,
              backgroundColor: AppColors.primary,
              child: Text(
                ad.isEmpty ? 'GP' : ad[0].toUpperCase(),
                style: const TextStyle(
                  color: Colors.white,
                  fontWeight: FontWeight.w700,
                ),
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Text(
          'Bugün hangi fırsatı yakalıyorsun?',
          style: textTheme.bodyMedium?.copyWith(color: AppColors.muted),
        ),
        const SizedBox(height: 20),
        TextField(
          readOnly: true,
          onTap: () {
            ScaffoldMessenger.of(context).showSnackBar(
              const SnackBar(
                content: Text('Arama ve filtreler Hafta 3’te eklenecek.'),
              ),
            );
          },
          decoration: const InputDecoration(
            hintText: 'Burs, staj, yarışma ara...',
            prefixIcon: Icon(Icons.search),
          ),
        ),
        const SizedBox(height: 24),
        Text(
          'Öne çıkan fırsatlar (${liste.length})',
          style: textTheme.titleLarge,
        ),
        const SizedBox(height: 12),
        for (final firsat in liste) ...[
          OpportunityCard(firsat: firsat),
          const SizedBox(height: 12),
        ],
      ],
    );
  }
}
