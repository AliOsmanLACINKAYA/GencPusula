import 'package:flutter/material.dart';

import '../../shared/models/firsat_ozeti.dart';
import '../theme/colors.dart';

class OpportunityCard extends StatelessWidget {
  const OpportunityCard({super.key, required this.firsat});

  final FirsatOzeti firsat;

  @override
  Widget build(BuildContext context) {
    return Card(
      child: ListTile(
        contentPadding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
        leading: CircleAvatar(
          backgroundColor: AppColors.primary.withValues(alpha: 0.10),
          child: const Icon(Icons.work_outline, color: AppColors.primary),
        ),
        title: Text(
          firsat.baslik,
          maxLines: 2,
          overflow: TextOverflow.ellipsis,
          style: Theme.of(context).textTheme.titleMedium,
        ),
        subtitle: Padding(
          padding: const EdgeInsets.only(top: 6),
          child: Text(
            '${firsat.firsatTuru} • ${firsat.sehir}\n'
            'Son başvuru: ${firsat.sonTarih}',
            style: Theme.of(context).textTheme.bodySmall
                ?.copyWith(color: AppColors.muted),
          ),
        ),
        trailing: const Icon(Icons.chevron_right),
        onTap: () {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Fırsat detayı Hafta 3’te eklenecek.'),
            ),
          );
        },
      ),
    );
  }
}
