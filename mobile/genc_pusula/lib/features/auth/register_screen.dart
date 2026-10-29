import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../core/utils/validators.dart';
import '../../core/widgets/button.dart';
import '../../core/widgets/scaffold.dart';
import '../../core/widgets/text_field.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _nameController = TextEditingController();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _passwordAgainController = TextEditingController();
  bool _obscurePassword = true;
  bool _isLoading = false;

  @override
  void dispose() {
    _nameController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    _passwordAgainController.dispose();
    super.dispose();
  }

  Future<void> _register() async {
    if (!_formKey.currentState!.validate()) return;

    setState(() => _isLoading = true);

    // Hafta 1: gerçek API yerine mock kayıt.
    await Future<void>.delayed(const Duration(milliseconds: 700));
    if (!mounted) return;

    setState(() => _isLoading = false);
    ScaffoldMessenger.of(context)
        .showSnackBar(const SnackBar(content: Text('Kayıt başarılı (mock).')));
    context.go('/home');
  }

  @override
  Widget build(BuildContext context) {
    return AuthScaffold(
      showBack: true,
      title: 'Aramıza katıl',
      subtitle: 'Sana uygun burs, staj ve etkinlikleri tek yerde takip et.',
      child: Form(
        key: _formKey,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppTextField(
              controller: _nameController,
              label: 'Ad soyad',
              prefixIcon: Icons.person_outline,
              textInputAction: TextInputAction.next,
              validator: (value) =>
                  validateRequired(value, message: 'Ad soyad gerekli'),
            ),
            const SizedBox(height: 14),
            AppTextField(
              controller: _emailController,
              label: 'E-posta',
              hint: 'ornek@universite.edu.tr',
              prefixIcon: Icons.mail_outline,
              keyboardType: TextInputType.emailAddress,
              textInputAction: TextInputAction.next,
              validator: validateEmail,
            ),
            const SizedBox(height: 14),
            AppTextField(
              controller: _passwordController,
              label: 'Parola',
              prefixIcon: Icons.lock_outline,
              obscureText: _obscurePassword,
              textInputAction: TextInputAction.next,
              validator: validatePassword,
              suffixIcon: IconButton(
                onPressed: () {
                  setState(() => _obscurePassword = !_obscurePassword);
                },
                icon: Icon(
                  _obscurePassword
                      ? Icons.visibility_outlined
                      : Icons.visibility_off_outlined,
                ),
              ),
            ),
            const SizedBox(height: 14),
            AppTextField(
              controller: _passwordAgainController,
              label: 'Parola tekrar',
              prefixIcon: Icons.lock_reset_outlined,
              obscureText: _obscurePassword,
              textInputAction: TextInputAction.done,
              onFieldSubmitted: (_) => _register(),
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Parola tekrarı gerekli';
                }
                if (value != _passwordController.text) {
                  return 'Parolalar eşleşmiyor';
                }
                return null;
              },
            ),
            const SizedBox(height: 20),
            AppButton(
              label: 'Kayıt ol',
              isLoading: _isLoading,
              onPressed: _register,
            ),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Text('Hesabın var mı?'),
                TextButton(
                  onPressed: () {
                    if (context.canPop()) {
                      context.pop();
                    } else {
                      context.go('/login');
                    }
                  },
                  child: const Text('Giriş yap'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
