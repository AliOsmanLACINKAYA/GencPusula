import 'package:flutter/material.dart';

import '../../core/app_state.dart';
import '../../core/theme/colors.dart';
import '../../core/widgets/button.dart';
import '../../core/widgets/etiket_badge.dart';
import '../../core/widgets/text_field.dart';
import '../../shared/models/firsat_ozeti.dart';

class AdminScreen extends StatefulWidget {
  const AdminScreen({super.key});

  @override
  State<AdminScreen> createState() => _AdminScreenState();
}

class _AdminScreenState extends State<AdminScreen> {
  final _form = GlobalKey<FormState>();
  final _baslik = TextEditingController();
  final _aciklama = TextEditingController();
  final _sehir = TextEditingController();
  final _kaynak = TextEditingController();
  final _tarih = TextEditingController();
  final _bolum = TextEditingController();
  final _sinif = TextEditingController();
  final _not = TextEditingController();
  final _dil = TextEditingController();

  List<FirsatOzeti> _firsatlar = [];
  List<String> _kategoriler = [];
  String? _kategori;
  DateTime? _sonTarih;
  bool _yukleniyor = true;
  bool _kaydediliyor = false;

  List<TextEditingController> get _kontroller => [
    _baslik,
    _aciklama,
    _sehir,
    _kaynak,
    _tarih,
    _bolum,
    _sinif,
    _not,
    _dil,
  ];

  @override
  void initState() {
    super.initState();
    _yukle();
  }

  @override
  void dispose() {
    for (final kontrol in _kontroller) {
      kontrol.dispose();
    }
    super.dispose();
  }

  Future<void> _yukle() async {
    setState(() => _yukleniyor = true);
    try {
      final firsatlar = await appState.firsatlariGetir();
      final kategoriler = await appState.kategorileriGetir();
      if (!mounted) return;
      setState(() {
        _firsatlar = firsatlar;
        _kategoriler = kategoriler;
      });
    } catch (e) {
      if (!mounted) return;
      _mesaj(_hata(e));
    } finally {
      if (mounted) setState(() => _yukleniyor = false);
    }
  }

  Future<void> _tarihSec() async {
    final bugun = DateTime.now();
    final tarih = await showDatePicker(
      context: context,
      initialDate: bugun.add(const Duration(days: 14)),
      firstDate: bugun,
      lastDate: bugun.add(const Duration(days: 365)),
    );
    if (tarih != null) {
      setState(() {
        _sonTarih = tarih;
        _tarih.text = tarih.toString().split(' ').first;
      });
    }
  }

  Future<void> _kaydet() async {
    if (!_form.currentState!.validate()) return;

    final baglanti = Uri.tryParse(_kaynak.text.trim());
    if (baglanti == null ||
        (baglanti.scheme != 'http' && baglanti.scheme != 'https') ||
        baglanti.host.isEmpty) {
      _mesaj('Geçerli bir bağlantı girin.');
      return;
    }

    setState(() => _kaydediliyor = true);
    try {
      await appState.firsatEkle({
        'firsatTuru': _kategori,
        'baslik': _baslik.text.trim(),
        'aciklama': _aciklama.text.trim(),
        'sehir': _sehir.text.trim(),
        'sonBasvuruTarihi': _sonTarih!.toIso8601String(),
        'kaynakBaglantisi': _kaynak.text.trim(),
        'gerekliBolum': _bolum.text.trim(),
        'gerekliSinif': _sinif.text.trim(),
        'minNotOrtalamasi': double.tryParse(_not.text.replaceAll(',', '.')),
        'gerekliDilSeviyesi': _dil.text.trim(),
      });
      if (!mounted) return;

      for (final kontrol in _kontroller) {
        kontrol.clear();
      }
      setState(() {
        _kategori = null;
        _sonTarih = null;
      });
      _mesaj('Fırsat eklendi.');
      await _yukle();
    } catch (e) {
      if (!mounted) return;
      _mesaj(_hata(e));
    } finally {
      if (mounted) setState(() => _kaydediliyor = false);
    }
  }

  String? _zorunlu(String? deger) =>
      deger == null || deger.trim().isEmpty ? 'Bu alan zorunludur.' : null;

  String _hata(Object hata) =>
      hata is ApiHatasi ? hata.mesaj : 'Bir hata oluştu. Tekrar dene.';

  void _mesaj(String metin) {
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(metin)));
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Yönetim'),
        actions: [
          IconButton(
            onPressed: _yukle,
            tooltip: 'Yenile',
            icon: const Icon(Icons.refresh),
          ),
        ],
      ),
      body: _yukleniyor
          ? const Center(child: CircularProgressIndicator())
          : ListView(
              padding: const EdgeInsets.all(16),
              children: [
                Text(
                  'Fırsatlar (${_firsatlar.length})',
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                const SizedBox(height: 8),
                for (final firsat in _firsatlar) _satir(firsat),
                const Divider(height: 32),
                Text(
                  'Yeni Fırsat Ekle',
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                const SizedBox(height: 12),
                _formAlani(),
              ],
            ),
    );
  }

  Widget _satir(FirsatOzeti firsat) {
    return ListTile(
      contentPadding: EdgeInsets.zero,
      title: Text(firsat.baslik, maxLines: 2, overflow: TextOverflow.ellipsis),
      subtitle: Wrap(
        spacing: 6,
        children: [
          if (firsat.onayDurumu.isNotEmpty)
            EtiketBadge(
              text: firsat.onayDurumu,
              color: firsat.onayDurumu == 'Onaylı'
                  ? AppColors.success
                  : AppColors.accent,
            ),
          if (firsat.guncellikEtiketi.isNotEmpty)
            EtiketBadge(
              text: firsat.guncellikEtiketi,
              color: AppColors.primary,
            ),
        ],
      ),
      trailing: Text(
        firsat.sonTarih,
        style: const TextStyle(fontSize: 12, color: AppColors.muted),
      ),
    );
  }

  Widget _alan(
    TextEditingController kontrol,
    String etiket, {
    String? ipucu,
    TextInputType? klavye,
    String? Function(String?)? dogrulama,
    int satir = 1,
  }) => Padding(
    padding: const EdgeInsets.only(bottom: 12),
    child: AppTextField(
      controller: kontrol,
      label: etiket,
      hint: ipucu,
      keyboardType: klavye,
      validator: dogrulama,
      maxLines: satir,
    ),
  );

  Widget _formAlani() {
    return Form(
      key: _form,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          DropdownButtonFormField<String>(
            key: ValueKey(_kategori ?? 'secilmedi'),
            initialValue: _kategori,
            decoration: const InputDecoration(labelText: 'Kategori'),
            items: [
              for (final kategori in _kategoriler)
                DropdownMenuItem(value: kategori, child: Text(kategori)),
            ],
            onChanged: (deger) => setState(() => _kategori = deger),
            validator: (deger) => deger == null ? 'Kategori seçin.' : null,
          ),
          const SizedBox(height: 12),
          _alan(_baslik, 'Başlık', dogrulama: _zorunlu),
          _alan(_aciklama, 'Açıklama', satir: 3, dogrulama: _zorunlu),
          _alan(_sehir, 'Şehir', dogrulama: _zorunlu),
          TextFormField(
            controller: _tarih,
            readOnly: true,
            decoration: const InputDecoration(
              labelText: 'Son başvuru tarihi',
              suffixIcon: Icon(Icons.calendar_today_outlined),
            ),
            onTap: _tarihSec,
            validator: (_) => _sonTarih == null ? 'Tarih seçin.' : null,
          ),
          const SizedBox(height: 12),
          _alan(
            _kaynak,
            'Kaynak bağlantısı',
            klavye: TextInputType.url,
            dogrulama: _zorunlu,
          ),
          ExpansionTile(
            title: const Text('Şartlar (isteğe bağlı)'),
            children: [
              _alan(
                _bolum,
                'Gerekli bölümler',
                ipucu: 'Bilgisayar Mühendisliği, Yazılım Mühendisliği',
              ),
              _alan(_sinif, 'Gerekli sınıflar', ipucu: '3. Sınıf, 4. Sınıf'),
              _alan(
                _not,
                'Minimum not ortalaması',
                klavye: TextInputType.number,
              ),
              _alan(_dil, 'Dil seviyesi', ipucu: 'B1'),
            ],
          ),
          AppButton(
            label: 'Fırsat Ekle',
            icon: Icons.add,
            isLoading: _kaydediliyor,
            onPressed: _kaydet,
          ),
        ],
      ),
    );
  }
}
