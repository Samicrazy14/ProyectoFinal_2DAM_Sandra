package com.example.appproyectofinal_sandra.activities

import android.content.Context
import android.content.Intent
import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentManager
import androidx.fragment.app.FragmentPagerAdapter
import androidx.viewpager.widget.ViewPager
import com.example.appproyectofinal_sandra.BooksAdminFragment
import com.example.appproyectofinal_sandra.databinding.ActivityDashBoardAdminBinding
import com.example.appproyectofinal_sandra.models.ModelCategory
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener
import java.util.ArrayList

class DashBoardAdminActivity : AppCompatActivity() {

    private lateinit var binding: ActivityDashBoardAdminBinding
    private lateinit var firebaseAuth: FirebaseAuth

    //Listado de categorias
    private lateinit var categoryArrayList: ArrayList<ModelCategory>
    private lateinit var viewPagerAdapter: ViewPagerAdapter

    //private lateinit var adapterCategory: AdapterCategory

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityDashBoardAdminBinding.inflate(layoutInflater)
        setContentView(binding.root)

        //init firebase auth
        firebaseAuth = FirebaseAuth.getInstance()

        checkUser()
        setupWithViewPagerAdapter(binding.viewPager)
        binding.tabLayout.setupWithViewPager(binding.viewPager)

        //Deslogearte
        binding.logoutBtn.setOnClickListener {
            firebaseAuth.signOut()
            checkUser()
        }

        //Ir a Perfil
        binding.profileBtn.setOnClickListener {
            startActivity(Intent(this, ProfileActivity::class.java))
        }
    }

    /**
     * Configura la página si es admin sino al MAIN
     */
    private fun checkUser() {
        val firebaseUser = firebaseAuth.currentUser
        if(firebaseUser == null){
            startActivity(Intent(this, MainActivity::class.java))
            finish()
        } else {
            val email = firebaseUser.email
            binding.subtitleTv.text = email
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

                //Categorias que se ven en el dashboard
                val modelCategories = ModelCategory("01", "Categories", 1, "")
                //Libros que se ven en el dashboard
                val modelBooks = ModelCategory("01", "Books", 1, "")
                //Usuarios que se ven en el dashboard
                val modelUsers = ModelCategory("01", "Users", 1, "")

                categoryArrayList.add(modelCategories)
                categoryArrayList.add(modelBooks)
                categoryArrayList.add(modelUsers)

                viewPagerAdapter.addFragment(
                    BooksAdminFragment.newInstance(
                        "${modelCategories.id}",
                        "${modelCategories.category}",
                        "${modelCategories.uid}"
                    ), modelCategories.category
                )

                viewPagerAdapter.addFragment(
                    BooksAdminFragment.newInstance(
                        "${modelBooks.id}",
                        "${modelBooks.category}",
                        "${modelBooks.uid}"
                    ), modelBooks.category
                )

                viewPagerAdapter.addFragment(
                    BooksAdminFragment.newInstance(
                        "${modelUsers.id}",
                        "${modelUsers.category}",
                        "${modelUsers.uid}"
                    ), modelUsers.category
                )

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
        private val fragmentList: ArrayList<BooksAdminFragment> = ArrayList()
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

        fun addFragment(fragment: BooksAdminFragment, title: String){
            fragmentList.add(fragment)
            fragmentTitleList.add(title)
        }
    }

}