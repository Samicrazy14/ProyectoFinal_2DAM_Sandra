package com.example.appproyectofinal_sandra.activities

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentManager
import androidx.fragment.app.FragmentPagerAdapter
import androidx.viewpager.widget.ViewPager
import com.example.appproyectofinal_sandra.BooksUserFragment
import com.example.appproyectofinal_sandra.databinding.ActivityDashBoardUserBinding
import com.example.appproyectofinal_sandra.models.ModelCategory
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener

class DashBoardUserActivity : AppCompatActivity() {

    private lateinit var binding: ActivityDashBoardUserBinding
    private lateinit var firebaseAuth: FirebaseAuth

    //Listado de categorias
    private lateinit var categoryArrayList: ArrayList<ModelCategory>
    private lateinit var viewPagerAdapter: ViewPagerAdapter

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityDashBoardUserBinding.inflate(layoutInflater)
        setContentView(binding.root)

        //init firebase auth
        firebaseAuth = FirebaseAuth.getInstance()

        //Config de la página dependiendo si es usuario o no
        checkUser()
        setupWithViewPagerAdapter(binding.viewPager)
        binding.tabLayout.setupWithViewPager(binding.viewPager)

        //Deslogearte
        binding.logoutBtn.setOnClickListener {
            firebaseAuth.signOut()
            startActivity(Intent(this, MainActivity::class.java))
            finish()
        }

        //Ir a Perfil
        binding.profileBtn.setOnClickListener {
            startActivity(Intent(this, ProfileActivity::class.java))
        }
    }

    /**
     * Configura la página en función si es usuario logado o no
     */
    private fun checkUser() {
        val firebaseUser = firebaseAuth.currentUser
        if(firebaseUser == null){
            binding.subtitleTv.text = "Usuario no registrado"
            binding.logoutBtn.visibility = View.GONE
            binding.profileBtn.visibility = View.GONE
        } else {
            val email = firebaseUser.email
            binding.subtitleTv.text = email
            binding.logoutBtn.visibility = View.VISIBLE
            binding.profileBtn.visibility = View.VISIBLE
        }
    }

    /**
     * Montaje de la paginación de categorías en la vista con fragments
     */
    private fun setupWithViewPagerAdapter(viewPager: ViewPager){
        //Adaptador de la vista del fragment
        viewPagerAdapter = ViewPagerAdapter(
            supportFragmentManager,
            FragmentPagerAdapter.BEHAVIOR_RESUME_ONLY_CURRENT_FRAGMENT,
            this
        )

        categoryArrayList = ArrayList()

        val ref = FirebaseDatabase.getInstance().getReference("Categories")
        ref.addListenerForSingleValueEvent(object : ValueEventListener {
            override fun onDataChange(snapshot: DataSnapshot) {
                categoryArrayList.clear()

                //Categroias que se ven en el dashboard
                val firebaseUser = firebaseAuth.currentUser

                val modelGutenberg = ModelCategory("01", "Obras Públicas", 1, "")


                if (firebaseUser == null) {
                    categoryArrayList.add(modelGutenberg)

                    viewPagerAdapter.addFragment(
                        BooksUserFragment.newInstance(
                            "${modelGutenberg.id}",
                            "${modelGutenberg.category}",
                            "${modelGutenberg.uid}"
                        ), modelGutenberg.category
                    )

                } else {

                    val modelAll = ModelCategory("01", "Novedades", 1, "")
                    val modelMostViewed = ModelCategory("01", "Most Viewed", 1, "")

                    categoryArrayList.add(modelAll)
                    categoryArrayList.add(modelMostViewed)
                    categoryArrayList.add(modelGutenberg)

                    viewPagerAdapter.addFragment(
                        BooksUserFragment.newInstance(
                            "${modelAll.id}",
                            "${modelAll.category}",
                            "${modelAll.uid}"
                        ), modelAll.category
                    )

                    viewPagerAdapter.addFragment(
                        BooksUserFragment.newInstance(
                            "${modelMostViewed.id}",
                            "${modelMostViewed.category}",
                            "${modelMostViewed.uid}"
                        ), modelMostViewed.category
                    )

                    viewPagerAdapter.addFragment(
                        BooksUserFragment.newInstance(
                            "${modelGutenberg.id}",
                            "${modelGutenberg.category}",
                            "${modelGutenberg.uid}"
                        ), modelGutenberg.category
                    )

                    for (ds in snapshot.children){
                        val model = ds.getValue(ModelCategory::class.java)

                        if (model != null){

                            categoryArrayList.add(model!!)

                            viewPagerAdapter.addFragment(
                                BooksUserFragment.newInstance(
                                    "${model.id}",
                                    "${model.category}",
                                    "${model.uid}"
                                ), model.category
                            )
                        }
                    }
                }

            viewPagerAdapter.notifyDataSetChanged()
            }

            override fun onCancelled(error: DatabaseError) {
            }
        })
        viewPager.adapter = viewPagerAdapter
    }

    /**
     * Adaptador de la vista del fragment
     */
    class ViewPagerAdapter(fm: FragmentManager, behavior: Int, context: Context): FragmentPagerAdapter(fm, behavior){
        private val fragmentList: ArrayList<BooksUserFragment> = ArrayList()
        private val  fragmentTitleList: ArrayList<String> = ArrayList()
        private val context: Context

        init {
            this.context = context
        }
        override fun getCount(): Int {
            return fragmentList.size
        }

        override fun getItem(position: Int): Fragment {
            return fragmentList[position]
        }

        override fun getPageTitle(position: Int): CharSequence? {
            return fragmentTitleList[position]
        }

        fun addFragment(fragment: BooksUserFragment, title: String){
            fragmentList.add(fragment)
            fragmentTitleList.add(title)
        }
    }



}