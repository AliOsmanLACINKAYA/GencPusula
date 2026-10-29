import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../core/widgets/empty_state.dart';

class SavedScreen extends StatelessWidget {
  const SavedScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Kaydedilenler')),
      body: EmptyState(
        icon: Icons.bookmark_border_rounded,
        title: 'Henüz kayıtlı fırsat yok',
        message:
            'İlgilendiğin fırsatları kaydederek burada takip edebilirsin. '
            'Fırsat kaydetme Hafta 4’te eklenecek.',
        actionLabel: 'Fırsatları keşfet',
        onAction: () => context.go('/home'),
      ),
    );
  }
}
