// app.dart
import 'package:flutter/material.dart';

import 'core/router/router.dart';
import 'core/theme/theme.dart';

class GencPusulaApp extends StatelessWidget {
  const GencPusulaApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'Genç Pusula',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.light,
      routerConfig: appRouter,
    );
  }
}
