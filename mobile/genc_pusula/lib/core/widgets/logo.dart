import 'package:flutter/material.dart';

import '../theme/colors.dart';

class AppLogo extends StatelessWidget {
  const AppLogo({super.key, this.onDark = false});

  final bool onDark;

  @override
  Widget build(BuildContext context) {
    final foreground = onDark ? Colors.white : AppColors.primary;

    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        Container(
          width: 44,
          height: 44,
          decoration: BoxDecoration(
            color: AppColors.accent,
            borderRadius: BorderRadius.circular(14),
          ),
          child: const Icon(
            Icons.explore_rounded,
            color: AppColors.primary,
            size: 28,
          ),
        ),
        const SizedBox(width: 12),
        Text(
          'Genç Pusula',
          style: Theme.of(context).textTheme.titleLarge?.copyWith(
            color: foreground,
            fontWeight: FontWeight.w800,
            letterSpacing: -0.2,
          ),
        ),
      ],
    );
  }
}
