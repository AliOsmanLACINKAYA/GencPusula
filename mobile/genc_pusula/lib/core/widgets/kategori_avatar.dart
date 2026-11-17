import 'package:flutter/material.dart';

import '../theme/colors.dart';

class KategoriAvatar extends StatelessWidget {
  const KategoriAvatar({super.key, required this.kategori});

  final String kategori;

  @override
  Widget build(BuildContext context) {
    return CircleAvatar(
      backgroundColor: Colors.transparent,
      child: Icon(_kategoriIkonu(kategori), color: AppColors.primary),
    );
  }

  IconData _kategoriIkonu(String kategori) {
    return switch (kategori) {
      'Burs' => Icons.school_outlined,
      'Staj' => Icons.work_outline,
      'Etkinlik' => Icons.event_outlined,
      'Yarisma' || 'Yarışma' => Icons.emoji_events_outlined,
      _ => Icons.work_outline,
    };
  }
}
