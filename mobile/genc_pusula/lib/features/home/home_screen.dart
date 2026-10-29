import 'package:flutter/material.dart';

import '../../core/theme/colors.dart';
import '../../core/widgets/opportunity.dart';
import '../../shared/mock/mock_firsatlar.dart';

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final textTheme = Theme.of(context).textTheme;

    return Scaffold(
      body: SafeArea(
        child: ListView(
          padding: const EdgeInsets.fromLTRB(20, 16, 20, 24),
          children: [
            Row(
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        'Merhaba, Kadirhan 👋',
                        style: textTheme.headlineSmall,
                      ),
                      const SizedBox(height: 4),
                      Text(
                        'Bugün hangi fırsatı yakalıyorsun?',
                        style: textTheme.bodyMedium?.copyWith(
                          color: AppColors.muted,
                        ),
                      ),
                    ],
                  ),
                ),
                const CircleAvatar(
                  radius: 22,
                  backgroundColor: AppColors.primary,
                  child: Text(
                    'KT',
                    style: TextStyle(
                      color: Colors.white,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 20),
            TextField(
              readOnly: true,
              onTap: () {
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text('Arama Hafta 3’te aktif olacak.'),
                  ),
                );
              },
              decoration: const InputDecoration(
                hintText: 'Burs, staj, yarışma ara...',
                prefixIcon: Icon(Icons.search),
              ),
            ),
            const SizedBox(height: 24),
            Row(
              children: [
                Expanded(
                  child: Text(
                    'Öne çıkan fırsatlar',
                    style: textTheme.titleLarge,
                  ),
                ),
                Text(
                  '${mockFirsatlar.length} fırsat',
                  style: textTheme.bodySmall?.copyWith(color: AppColors.muted),
                ),
              ],
            ),
            const SizedBox(height: 12),
            for (final firsat in mockFirsatlar) ...[
              OpportunityCard(firsat: firsat),
              const SizedBox(height: 12),
            ],
          ],
        ),
      ),
    );
  }
}
